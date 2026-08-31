using KurumiConcursos.ApplicationService.DataTransferObjects.JourneyDtos.Request;
using KurumiConcursos.ApplicationService.DataTransferObjects.JourneyDtos.Response;
using KurumiConcursos.ApplicationService.Interfaces.MapperContracts;
using KurumiConcursos.ApplicationService.Interfaces.ServiceContracts;
using KurumiConcursos.ApplicationService.Traces;
using KurumiConcursos.Domain.Entities;
using KurumiConcursos.Domain.Enums;
using KurumiConcursos.Domain.Interface;
using KurumiConcursos.Domain.ValueObjects;
using KurumiConcursos.Infra.Interfaces.RepositoryContracts;

namespace KurumiConcursos.ApplicationService.Services.SyllabusNodeStudyServices;

public sealed class SyllabusNodeStudyCommandService(
    IJourneyRepository journeyRepository,
    IFocusSessionRepository focusSessionRepository,
    IReviewAppointmentRepository reviewAppointmentRepository,
    IStudyRoutineBlockRepository studyRoutineBlockRepository,
    ISyllabusNodeStudyMapper mapper,
    IValidate<SyllabusNode> validation,
    INotificationHandler notification,
    ILoggerHandler logger)
    : ServiceBase<SyllabusNode>(notification, validation, logger), ISyllabusNodeStudyCommandService
{
    public async Task<SyllabusNodeStudyResponse?> SaveAsync(
        SyllabusNodeStudyRequest request,
        UserCredential credential)
    {
        if (request.JourneyId <= 0 || request.SyllabusNodeId <= 0)
        {
            Notification.CreateNotification(SyllabusNodeStudyTrace.Save, "Topico invalido.");
            return null;
        }

        if (request.StudiedMinutes < 0)
        {
            Notification.CreateNotification(SyllabusNodeStudyTrace.Save, "O tempo estudado nao pode ser negativo.");
            return null;
        }

        var journey = await journeyRepository.FindByIdAsync(
            request.JourneyId,
            credential.UserId,
            CancellationToken.None,
            includeStructure: true,
            tracking: true);
        var node = journey?.KnowledgeAreas
            .SelectMany(area => area.SyllabusNodes)
            .FirstOrDefault(item => item.Id == request.SyllabusNodeId);

        if (node is null)
        {
            Notification.CreateNotification(SyllabusNodeStudyTrace.Save, "Topico nao encontrado.");
            return null;
        }

        // Pendente e nao iniciado sao estados diferentes. Quando o usuario
        // desfaz uma pendencia, a acao nao deve voltar a marcar o no como
        // InProgress nem criar uma nova sessao.
        var clearPending = request.ClearPending;
        if (clearPending)
        {
            request = request with
            {
                Completed = false,
                StudiedMinutes = 0,
                ScheduleReview = false,
                ReviewDate = null
            };
        }

        var wasCompleted = node.Progress == EStudyProgress.Studied;
        if (request.Completed && request.StudiedMinutes == 0 && !wasCompleted)
        {
            Notification.CreateNotification(
                SyllabusNodeStudyTrace.Save,
                "Informe o tempo utilizado para concluir o topico.");
            return null;
        }

        // Agendar revisao de um topico ja concluido nao cria uma nova sessao de estudo.
        var shouldRecordStudy = request.StudiedMinutes > 0 &&
                                !(wasCompleted && request.Completed && request.ScheduleReview);

        node.Progress = clearPending
            ? EStudyProgress.NotStarted
            : request.Completed
                ? EStudyProgress.Studied
                : EStudyProgress.InProgress;
        var today = CurrentDate();
        if (clearPending)
        {
            node.StudyStartedOn = null;
            node.StudiedOn = null;
        }
        else if (request.Completed || request.StudiedMinutes > 0 || node.StudyStartedOn.HasValue)
            node.StudyStartedOn ??= today;

        node.StudiedOn = request.Completed ? today : null;
        node.LastUpdateDate = DateTimeOffset.UtcNow;

        var allNodes = journey!.KnowledgeAreas
            .SelectMany(area => area.SyllabusNodes)
            .ToList();
        var rootNode = FindRootNode(node, allNodes);
        var rootChildren = allNodes
            .Where(item => item.ParentId == rootNode.Id)
            .ToList();
        var descendants = FindDescendants(rootNode.Id, allNodes);

        // A conclusao do topico-pai conclui toda a sua arvore de subtópicos.
        if (node.Id == rootNode.Id && rootChildren.Count > 0)
        {
            foreach (var descendant in descendants)
            {
                descendant.Progress = clearPending
                    ? EStudyProgress.NotStarted
                    : request.Completed
                        ? EStudyProgress.Studied
                        : descendant.StudyStartedOn.HasValue
                            ? EStudyProgress.InProgress
                            : EStudyProgress.NotStarted;
                if (clearPending)
                {
                    descendant.StudyStartedOn = null;
                    descendant.StudiedOn = null;
                }
                else if (request.Completed)
                    descendant.StudyStartedOn ??= today;

                descendant.StudiedOn = request.Completed ? today : null;
                descendant.LastUpdateDate = DateTimeOffset.UtcNow;
            }
        }

        if (rootChildren.Count > 0)
        {
            var completedChildren = rootChildren.Count(item => item.Progress == EStudyProgress.Studied);
            rootNode.Progress = completedChildren == rootChildren.Count
                ? EStudyProgress.Studied
                : completedChildren > 0 || rootChildren.Any(item => item.Progress == EStudyProgress.InProgress)
                    ? EStudyProgress.InProgress
                    : EStudyProgress.NotStarted;
            if (clearPending)
                rootNode.StudyStartedOn = null;
            else
                rootNode.StudyStartedOn ??= node.StudyStartedOn;
            rootNode.StudiedOn = rootNode.Progress == EStudyProgress.Studied
                ? CurrentDate()
                : null;
            rootNode.LastUpdateDate = DateTimeOffset.UtcNow;
        }

        if (!await EntityValidationAsync(node))
            return null;

        if (!await journeyRepository.UpdateNodeAsync(node))
        {
            Notification.CreateNotification(
                SyllabusNodeStudyTrace.Save,
                "Nao foi possivel salvar o progresso.");
            return null;
        }

        if (shouldRecordStudy)
        {
            var session = mapper.DtoToFocusSession(
                credential.UserId,
                node,
                request,
                CurrentDate());
            if (!await focusSessionRepository.SaveAsync(session))
            {
                Notification.CreateNotification(
                    SyllabusNodeStudyTrace.Save,
                    "Nao foi possivel salvar o tempo estudado.");
                return null;
            }
        }

        var reviewNodeIds = node.Id == rootNode.Id && rootChildren.Count > 0
            ? descendants.Select(item => item.Id).Append(rootNode.Id).ToHashSet()
            : new HashSet<long> { node.Id };
        var appointments = await reviewAppointmentRepository.FindAllAsync(item =>
            item.UserId == credential.UserId &&
            reviewNodeIds.Contains(item.SyllabusNodeId) &&
            !item.Completed &&
            !item.Superseded);
        foreach (var appointment in appointments)
        {
            appointment.Superseded = true;
            appointment.LastUpdateDate = DateTimeOffset.UtcNow;
            await reviewAppointmentRepository.UpdateAsync(appointment);
        }

        if (request.Completed && request.ScheduleReview && request.ReviewDate.HasValue)
        {
            if (request.ReviewDate.Value < CurrentDate())
            {
                Notification.CreateNotification(
                    SyllabusNodeStudyTrace.Save,
                    "A data da revisao deve ser futura.");
                return null;
            }

            await reviewAppointmentRepository.SaveAsync(
                mapper.DtoToReviewAppointment(
                    credential.UserId,
                    node,
                    request.ReviewDate.Value));
        }

        if (!await SyncStudyRoutineBlockAsync(
                request,
                credential.UserId,
                rootNode,
                rootChildren,
                shouldRecordStudy ? request.StudiedMinutes : 0,
                clearPending))
            return null;

        GenerateLogger(
            EUserAction.Update,
            SyllabusNodeStudyTrace.Save,
            credential.UserId,
            node);
        var minutes = await GetStudiedMinutesAsync(node.Id, credential.UserId);
        var reviewDate = await GetReviewDateAsync(node.Id, credential.UserId);
        return mapper.DomainToDtoResponse(node, minutes, reviewDate);
    }

    private async Task<int> GetStudiedMinutesAsync(long nodeId, Guid userId)
    {
        var sessions = await focusSessionRepository.FindAllAsync(item =>
            item.UserId == userId && item.SyllabusNodeId == nodeId);
        return sessions.Sum(item => item.DurationSeconds) / 60;
    }

    private async Task<DateOnly?> GetReviewDateAsync(long nodeId, Guid userId)
    {
        var appointments = await reviewAppointmentRepository.FindAllAsync(item =>
            item.UserId == userId &&
            item.SyllabusNodeId == nodeId &&
            !item.Completed &&
            !item.Superseded);
        return appointments.Count == 0 ? null : appointments.Min(item => item.ScheduledFor);
    }

    private async Task<bool> SyncStudyRoutineBlockAsync(
        SyllabusNodeStudyRequest request,
        Guid userId,
        SyllabusNode rootNode,
        IReadOnlyCollection<SyllabusNode> rootChildren,
        int recordedMinutes,
        bool clearPending)
    {
        var today = CurrentDate();
        var block = (await studyRoutineBlockRepository.FindAllAsync(item =>
                item.UserId == userId &&
                item.JourneyId == request.JourneyId &&
                item.SyllabusNodeId == rootNode.Id &&
                item.ScheduledFor == today &&
                item.Type == EStudyBlockType.Study))
            .OrderByDescending(item => item.StudyRoutineId)
            .ThenBy(item => item.Order)
            .FirstOrDefault();

        if (block is null)
            return true;

        if (clearPending)
            block.CompletedMinutes = 0;
        else if (recordedMinutes > 0)
            block.CompletedMinutes += recordedMinutes;

        var completed = rootChildren.Count > 0
            ? rootChildren.All(item => item.Progress == EStudyProgress.Studied)
            : request.Completed;
        block.Status = completed ? EStudyBlockStatus.Completed : EStudyBlockStatus.Pending;
        block.CompletedAt = completed ? DateTimeOffset.UtcNow : null;
        block.LastUpdateDate = DateTimeOffset.UtcNow;

        if (await studyRoutineBlockRepository.UpdateAsync(block))
            return true;

        return Notification.CreateNotification(
            SyllabusNodeStudyTrace.Save,
            "Nao foi possivel atualizar o bloco do plano de estudos.");
    }

    private static SyllabusNode FindRootNode(
        SyllabusNode node,
        IReadOnlyCollection<SyllabusNode> allNodes)
    {
        var byId = allNodes.ToDictionary(item => item.Id);
        var current = node;

        while (current.ParentId.HasValue && byId.TryGetValue(current.ParentId.Value, out var parent))
            current = parent;

        return current;
    }

    private static List<SyllabusNode> FindDescendants(
        long rootId,
        IReadOnlyCollection<SyllabusNode> allNodes)
    {
        var descendants = new List<SyllabusNode>();
        var pending = new Queue<long>([rootId]);

        while (pending.Count > 0)
        {
            var parentId = pending.Dequeue();
            var children = allNodes.Where(item => item.ParentId == parentId).ToList();
            descendants.AddRange(children);
            foreach (var child in children)
                pending.Enqueue(child.Id);
        }

        return descendants;
    }

    private static DateOnly CurrentDate()
    {
        var zone = TimeZoneInfo.FindSystemTimeZoneById(
            OperatingSystem.IsWindows() ? "E. South America Standard Time" : "America/Sao_Paulo");
        return DateOnly.FromDateTime(TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow, zone));
    }
}