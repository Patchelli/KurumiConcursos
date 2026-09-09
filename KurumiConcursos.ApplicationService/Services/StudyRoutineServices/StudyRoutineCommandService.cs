using System.Text.Json;
using KurumiConcursos.ApplicationService.DataTransferObjects.StudyRoutineBlockDtos.Request;
using KurumiConcursos.ApplicationService.DataTransferObjects.StudyRoutineBlockDtos.Response;
using KurumiConcursos.ApplicationService.DataTransferObjects.StudyRoutineDtos.Request;
using KurumiConcursos.ApplicationService.DataTransferObjects.StudyRoutineDtos.Response;
using KurumiConcursos.ApplicationService.Interfaces.MapperContracts;
using KurumiConcursos.ApplicationService.Interfaces.ServiceContracts;
using KurumiConcursos.ApplicationService.Traces;
using KurumiConcursos.Domain.Entities;
using KurumiConcursos.Domain.Enums;
using KurumiConcursos.Domain.Interface;
using KurumiConcursos.Domain.ValueObjects;
using KurumiConcursos.Infra.Interfaces.RepositoryContracts;

namespace KurumiConcursos.ApplicationService.Services.StudyRoutineServices;

public sealed class StudyRoutineCommandService(
    IStudyRoutineRepository studyRoutineRepository,
    IJourneyRepository journeyRepository,
    IStudyRoutineMapper studyRoutineMapper,
    IStudyRoutineBlockRepository studyRoutineBlockRepository,
    IFocusSessionRepository focusSessionRepository,
    IStudySummaryRepository studySummaryRepository,
    IReviewAppointmentRepository reviewAppointmentRepository,
    IQuestionAppointmentCommandService questionAppointmentCommandService,
    ITimeCapsuleCommandService timeCapsuleCommandService,
    IValidate<StudyRoutine> studyRoutineValidation,
    INotificationHandler notificationHandler,
    ILoggerHandler logger)
    : ServiceBase<StudyRoutine>(notificationHandler, studyRoutineValidation, logger),
        IStudyRoutineCommandService
{
    private static readonly JsonSerializerOptions JsonOptions = new(JsonSerializerDefaults.Web);
    private static readonly SemaphoreSlim GenerationLock = new(1, 1);

    public async Task<StudyRoutineResponse?> RegisterAsync(
        StudyRoutineRegisterRequest request,
        UserCredential credential)
    {
        if (request.JourneyId <= 0)
        {
            Notification.CreateNotification(StudyRoutineTrace.Register, "Jornada invalida.");
            return null;
        }

        if (await journeyRepository.FindByIdAsync(
                request.JourneyId,
                credential.UserId,
                CancellationToken.None) is null)
        {
            Notification.CreateNotification(StudyRoutineTrace.Register, "Jornada nao encontrada.");
            return null;
        }

        var studyRoutine = studyRoutineMapper.DtoRegisterToDomain(credential.UserId, request);
        if (!await EntityValidationAsync(studyRoutine))
            return null;

        if (!await studyRoutineRepository.SaveAsync(studyRoutine))
        {
            Notification.CreateNotification(
                StudyRoutineTrace.Register,
                "Nao foi possivel cadastrar o plano de estudos.");
            return null;
        }

        GenerateLogger(
            EUserAction.Save,
            StudyRoutineTrace.Register,
            credential.UserId,
            studyRoutine.Id.ToString());

        return studyRoutineMapper.DomainToDtoResponse(studyRoutine);
    }

    public async Task<StudyRoutineResponse?> UpdateAsync(
        StudyRoutineUpdateRequest request,
        UserCredential credential)
    {
        if (request.Id <= 0)
        {
            Notification.CreateNotification(StudyRoutineTrace.Update, "Plano de estudos invalido.");
            return null;
        }

        var studyRoutine = await studyRoutineRepository.FindByPredicateAsync(
            item => item.Id == request.Id && item.UserId == credential.UserId,
            asNoTracking: false);
        if (studyRoutine is null)
        {
            Notification.CreateNotification(StudyRoutineTrace.Update, "Plano de estudos nao encontrado.");
            return null;
        }

        studyRoutineMapper.DtoUpdateToDomain(studyRoutine, request);
        if (!await EntityValidationAsync(studyRoutine))
            return null;

        if (!await studyRoutineRepository.UpdateAsync(studyRoutine))
        {
            Notification.CreateNotification(
                StudyRoutineTrace.Update,
                "Nao foi possivel atualizar o plano de estudos.");
            return null;
        }

        GenerateLogger(
            EUserAction.Update,
            StudyRoutineTrace.Update,
            credential.UserId,
            studyRoutine.Id.ToString());

        return studyRoutineMapper.DomainToDtoResponse(studyRoutine);
    }

    public async Task<IList<StudyRoutineBlockResponse>> GenerateAsync(
        StudyRoutineGenerateRequest request,
        UserCredential credential)
    {
        if (request.StudyRoutineId <= 0 || request.JourneyId <= 0)
        {
            Notification.CreateNotification(StudyRoutineTrace.Generate, "Plano de estudos invalido.");
            return [];
        }

        await GenerationLock.WaitAsync();
        try
        {
            var studyRoutine = await studyRoutineRepository.FindByPredicateAsync(
                item => item.Id == request.StudyRoutineId && item.UserId == credential.UserId,
                asNoTracking: true);
            if (studyRoutine is null)
            {
                Notification.CreateNotification(StudyRoutineTrace.Generate, "Plano de estudos nao encontrado.");
                return [];
            }

            var journey = await journeyRepository.FindByIdAsync(
                request.JourneyId,
                credential.UserId,
                CancellationToken.None,
                includeStructure: true);
            if (journey is null || journey.Id != studyRoutine.JourneyId)
            {
                Notification.CreateNotification(StudyRoutineTrace.Generate, "Jornada do plano nao encontrada.");
                return [];
            }

            var configuration = JsonSerializer.Deserialize<StudyRoutineConfigurationRequest>(
                                    studyRoutine.ConfigurationJson,
                                    JsonOptions)
                                ?? new StudyRoutineConfigurationRequest(
                                    [],
                                    new Dictionary<long, string>(),
                                    1,
                                    new Dictionary<string, decimal>(),
                                    new Dictionary<long, decimal>(),
                                    new Dictionary<long, decimal>());
            var today = CurrentDate();
            var futureBlocks = await studyRoutineBlockRepository.FindAllAsync(item =>
                item.StudyRoutineId == studyRoutine.Id &&
                item.UserId == credential.UserId &&
                item.ScheduledFor >= today &&
                item.Status == EStudyBlockStatus.Pending);
            foreach (var futureBlock in futureBlocks)
                await studyRoutineBlockRepository.DeleteAsync(futureBlock);

            var selectedAreaIds = (configuration.KnowledgeAreaIds ?? []).ToHashSet();
            var affinity = configuration.Affinity ?? new Dictionary<long, string>();
            var availability = configuration.Availability ?? new Dictionary<string, decimal>();
            var areaOverrides = configuration.AreaHoursOverride ?? new Dictionary<long, decimal>();
            var nodeOverrides = configuration.NodeHoursOverride ?? new Dictionary<long, decimal>();
            var orderedAreas = (journey.KnowledgeAreas ?? [])
                .Where(area => selectedAreaIds.Count == 0 || selectedAreaIds.Contains(area.Id))
                .OrderByDescending(area => Priority(affinity.GetValueOrDefault(area.Id)))
                .ThenBy(area => area.Order)
                .Select(area => new
                {
                    Area = area,
                    Nodes = (area.SyllabusNodes ?? [])
                        .Where(node => node.ParentId is null && node.Progress != EStudyProgress.Studied)
                        .OrderBy(node => node.Order)
                        .ToList()
                })
                .Where(item => item.Nodes.Count > 0)
                .ToList();

            var nodes = new List<(SyllabusNode Node, int Minutes)>();
            for (var round = 0; orderedAreas.Any(item => round < item.Nodes.Count); round++)
            {
                foreach (var area in orderedAreas)
                {
                    if (round >= area.Nodes.Count)
                        continue;

                    var node = area.Nodes[round];
                    var hours = nodeOverrides.GetValueOrDefault(
                        node.Id,
                        areaOverrides.GetValueOrDefault(area.Area.Id, configuration.HoursPerTopic));
                    nodes.Add((node, (int)Math.Max(1, Math.Round((double)hours * 60))));
                }
            }

            if (nodes.Count == 0)
                return [];

            var result = new List<StudyRoutineBlockResponse>();
            var cursor = 0;
            var order = 0;
            var date = today;
            for (var day = 0; day < 120 && cursor < nodes.Count; day++, date = date.AddDays(1))
            {
                var key = date.DayOfWeek switch
                {
                    DayOfWeek.Monday => "SEG",
                    DayOfWeek.Tuesday => "TER",
                    DayOfWeek.Wednesday => "QUA",
                    DayOfWeek.Thursday => "QUI",
                    DayOfWeek.Friday => "SEX",
                    DayOfWeek.Saturday => "SÁB",
                    _ => "DOM"
                };
                var available = (int)Math.Max(
                    0,
                    Math.Round((double)availability.GetValueOrDefault(key)) * 60);
                var studyBudget = available;
                while (studyBudget > 0 && cursor < nodes.Count)
                {
                    var item = nodes[cursor];
                    var used = Math.Min(studyBudget, item.Minutes);
                    var block = new StudyRoutineBlock
                    {
                        UserId = credential.UserId,
                        JourneyId = studyRoutine.JourneyId,
                        StudyRoutineId = studyRoutine.Id,
                        SyllabusNodeId = item.Node.Id,
                        ScheduledFor = date,
                        Type = EStudyBlockType.Study,
                        Status = EStudyBlockStatus.Pending,
                        PlannedMinutes = used,
                        Order = order++
                    };
                    if (!await studyRoutineBlockRepository.SaveAsync(block))
                    {
                        Notification.CreateNotification(
                            StudyRoutineTrace.Generate,
                            "Nao foi possivel gerar um bloco do plano de estudos.");
                        break;
                    }

                    result.Add(ToResponse(block));
                    studyBudget -= used;
                    nodes[cursor] = (item.Node, item.Minutes - used);
                    if (nodes[cursor].Minutes == 0)
                        cursor++;
                }
            }

            GenerateLogger(
                EUserAction.Save,
                StudyRoutineTrace.Generate,
                credential.UserId,
                studyRoutine.Id.ToString());
            return result;
        }
        finally
        {
            GenerationLock.Release();
        }
    }

    public async Task<StudyRoutineBlockResponse?> CompleteBlockAsync(
        StudyRoutineBlockCompleteRequest request,
        UserCredential credential)
    {
        if (request.BlockId <= 0)
        {
            Notification.CreateNotification(StudyRoutineTrace.CompleteBlock, "Bloco invalido.");
            return null;
        }

        if (request.StudyLocation?.Trim().Length > 200)
        {
            Notification.CreateNotification(StudyRoutineTrace.CompleteBlock, "O local de estudo deve ter no maximo 200 caracteres.");
            return null;
        }

        var block = await studyRoutineBlockRepository.FindByPredicateAsync(
            item => item.Id == request.BlockId && item.UserId == credential.UserId,
            asNoTracking: false);
        if (block is null)
        {
            Notification.CreateNotification(StudyRoutineTrace.CompleteBlock, "Bloco nao encontrado.");
            return null;
        }

        if (request.ClearPending)
            request = request with { Completed = false, CompletedMinutes = 0, ScheduleReview = false, ReviewDate = null };

        var resetStudy = request.ClearPending || !request.Completed && request.CompletedMinutes == 0;
        var removeRecordedStudy = resetStudy;
        var affectedNodeIds = new HashSet<long> { block.SyllabusNodeId };

        if (block.Type == EStudyBlockType.Study)
        {
            var journey = await journeyRepository.FindByIdAsync(
                block.JourneyId,
                credential.UserId,
                CancellationToken.None,
                includeStructure: true,
                tracking: true);
            var rootNode = journey?.KnowledgeAreas
                .SelectMany(area => area.SyllabusNodes)
                .FirstOrDefault(node => node.Id == block.SyllabusNodeId);
            if (rootNode is null)
            {
                Notification.CreateNotification(StudyRoutineTrace.CompleteBlock, "Topico do bloco nao encontrado.");
                return null;
            }

            var allNodes = journey!.KnowledgeAreas.SelectMany(area => area.SyllabusNodes).ToList();
            var descendants = FindDescendants(rootNode.Id, allNodes);
            affectedNodeIds.UnionWith(descendants.Select(item => item.Id));
            var today = CurrentDate();
            if (!request.Completed || rootNode.Progress != EStudyProgress.Studied)
                rootNode.LastStudyLocation = null;
            rootNode.Progress = resetStudy
                ? EStudyProgress.NotStarted
                : request.Completed
                    ? EStudyProgress.Studied
                    : EStudyProgress.InProgress;
            if (resetStudy)
            {
                rootNode.StudyStartedOn = null;
                rootNode.StudiedOn = null;
            }
            else if (request.Completed)
            {
                rootNode.StudyStartedOn ??= today;
                rootNode.StudiedOn = today;
            }
            else
            {
                rootNode.StudyStartedOn ??= today;
                rootNode.StudiedOn = null;
            }

            rootNode.LastUpdateDate = DateTimeOffset.UtcNow;
            if (request.Completed && !string.IsNullOrWhiteSpace(request.StudyLocation))
                rootNode.LastStudyLocation = request.StudyLocation.Trim();

            foreach (var descendant in descendants)
            {
                // Salvar tempo parcial do pai não altera os subtópicos.
                if (!request.Completed && !resetStudy) continue;
                if (!request.Completed || descendant.Progress != EStudyProgress.Studied)
                    descendant.LastStudyLocation = null;
                descendant.Progress = resetStudy
                    ? EStudyProgress.NotStarted
                    : EStudyProgress.Studied;
                if (resetStudy)
                {
                    descendant.StudyStartedOn = null;
                    descendant.StudiedOn = null;
                }
                else if (request.Completed)
                {
                    descendant.StudyStartedOn ??= today;
                    descendant.StudiedOn = today;
                }
                else
                {
                    descendant.StudiedOn = null;
                }

                descendant.LastUpdateDate = DateTimeOffset.UtcNow;
                if (request.Completed && !string.IsNullOrWhiteSpace(request.StudyLocation))
                    descendant.LastStudyLocation = request.StudyLocation.Trim();
            }

            if (!await journeyRepository.UpdateNodeAsync(rootNode))
            {
                Notification.CreateNotification(
                    StudyRoutineTrace.CompleteBlock,
                    "Nao foi possivel atualizar o progresso do topico.");
                return null;
            }
        }

        if (block.Type == EStudyBlockType.Review && request.Completed && !string.IsNullOrWhiteSpace(request.StudyLocation))
        {
            var node = await journeyRepository.FindNodeAsync(block.SyllabusNodeId, credential.UserId,
                CancellationToken.None, false);
            if (node is null)
            {
                Notification.CreateNotification(StudyRoutineTrace.CompleteBlock, "Topico do bloco nao encontrado.");
                return null;
            }
            node.LastStudyLocation = request.StudyLocation.Trim();
            node.LastUpdateDate = DateTimeOffset.UtcNow;
            if (!await journeyRepository.UpdateNodeAsync(node))
            {
                Notification.CreateNotification(StudyRoutineTrace.CompleteBlock, "Nao foi possivel salvar o local de estudo.");
                return null;
            }
        }

        if (request.ClearPending || !request.Completed && request.CompletedMinutes == 0)
            block.CompletedMinutes = 0;
        else if (request.CompletedMinutes > 0)
            block.CompletedMinutes += request.CompletedMinutes;
        block.Status = request.Completed
            ? EStudyBlockStatus.Completed
            : EStudyBlockStatus.Pending;
        block.CompletedAt = request.Completed ? DateTimeOffset.UtcNow : null;
        block.LastUpdateDate = DateTimeOffset.UtcNow;
        if (!await studyRoutineBlockRepository.UpdateAsync(block))
        {
            Notification.CreateNotification(
                StudyRoutineTrace.CompleteBlock,
                "Nao foi possivel atualizar o bloco do plano de estudos.");
            return null;
        }

        if (removeRecordedStudy)
        {
            var recordedSessions = await focusSessionRepository.FindAllAsync(item =>
                item.UserId == credential.UserId &&
                item.JourneyId == block.JourneyId &&
                item.SyllabusNodeId.HasValue &&
                affectedNodeIds.Contains(item.SyllabusNodeId.Value));
            foreach (var recordedSession in recordedSessions)
                if (!await focusSessionRepository.DeleteAsync(recordedSession))
                {
                    Notification.CreateNotification(
                        StudyRoutineTrace.CompleteBlock,
                        "Nao foi possivel remover o tempo estudado.");
                    return null;
                }
        }

        if (request.Completed && !string.IsNullOrWhiteSpace(request.Summary))
            await studySummaryRepository.SaveAsync(new StudySummary
            {
                UserId = credential.UserId, JourneyId = block.JourneyId,
                SyllabusNodeId = block.SyllabusNodeId,
                IsReview = block.Type == EStudyBlockType.Review,
                Content = request.Summary.Trim()
            });

        if (block.Type is EStudyBlockType.Study or EStudyBlockType.Review)
        {
            if (!await questionAppointmentCommandService.SupersedePendingAsync(
                    credential.UserId, affectedNodeIds))
            {
                Notification.CreateNotification(StudyRoutineTrace.CompleteBlock,
                    "Nao foi possivel atualizar o agendamento de questoes.");
                return null;
            }

            if (request.Completed)
            {
                var node = await journeyRepository.FindNodeAsync(
                    block.SyllabusNodeId, credential.UserId, CancellationToken.None);
                if (node is null || !await questionAppointmentCommandService.ScheduleAsync(
                        credential.UserId, block.JourneyId, node, CurrentDate(),
                        request.ScheduleReview ? request.ReviewDate : null))
                {
                    Notification.CreateNotification(StudyRoutineTrace.CompleteBlock,
                        "Nao foi possivel agendar as questoes.");
                    return null;
                }
            }
        }

        if (block.Type == EStudyBlockType.Study)
        {
            var reviews = await studyRoutineBlockRepository.FindAllAsync(item =>
                item.UserId == credential.UserId &&
                item.StudyRoutineId == block.StudyRoutineId &&
                item.SyllabusNodeId == block.SyllabusNodeId &&
                item.Type == EStudyBlockType.Review &&
                item.ScheduledFor >= CurrentDate());
            foreach (var review in reviews)
                await studyRoutineBlockRepository.DeleteAsync(review);

            if (!request.Completed)
            {
                var appointments = await reviewAppointmentRepository.FindAllAsync(item =>
                    item.UserId == credential.UserId &&
                    affectedNodeIds.Contains(item.SyllabusNodeId) &&
                    !item.Completed && !item.Superseded);
                foreach (var appointment in appointments)
                {
                    appointment.Superseded = true;
                    appointment.LastUpdateDate = DateTimeOffset.UtcNow;
                    await reviewAppointmentRepository.UpdateAsync(appointment);
                }
            }
        }

        if ((block.Type == EStudyBlockType.Study || block.Type == EStudyBlockType.Review) &&
            request.Completed &&
            request.ScheduleReview &&
            request.ReviewDate.HasValue)
        {
            await studyRoutineBlockRepository.SaveAsync(new StudyRoutineBlock
            {
                UserId = credential.UserId,
                JourneyId = block.JourneyId,
                StudyRoutineId = block.StudyRoutineId,
                SyllabusNodeId = block.SyllabusNodeId,
                ScheduledFor = request.ReviewDate.Value,
                Type = EStudyBlockType.Review,
                Status = EStudyBlockStatus.Pending,
                PlannedMinutes = 0,
                Order = block.Order
            });
        }

        GenerateLogger(
            EUserAction.Update,
            StudyRoutineTrace.CompleteBlock,
            credential.UserId,
            block.Id.ToString());
        await timeCapsuleCommandService.EvaluateJourneyTriggersAsync(credential.UserId, block.JourneyId);
        return ToResponse(block);
    }

    private static int Priority(string? affinity) => affinity?.ToLowerInvariant() switch
    {
        "muito baixa" => 5,
        "baixa" => 4,
        "alta" => 2,
        "muito alta" => 1,
        _ => 3
    };

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

    private static StudyRoutineBlockResponse ToResponse(StudyRoutineBlock block) => new(
        block.Id,
        block.SyllabusNodeId,
        block.ScheduledFor,
        block.Type,
        block.Status,
        block.PlannedMinutes,
        block.CompletedMinutes,
        block.Order);
}
