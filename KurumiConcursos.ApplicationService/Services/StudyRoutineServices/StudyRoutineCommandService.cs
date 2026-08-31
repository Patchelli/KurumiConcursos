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
                                    7,
                                    50,
                                    25,
                                    25,
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

        var block = await studyRoutineBlockRepository.FindByPredicateAsync(
            item => item.Id == request.BlockId && item.UserId == credential.UserId,
            asNoTracking: false);
        if (block is null)
        {
            Notification.CreateNotification(StudyRoutineTrace.CompleteBlock, "Bloco nao encontrado.");
            return null;
        }

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
            var today = CurrentDate();
            rootNode.Progress = request.ClearPending
                ? EStudyProgress.NotStarted
                : request.Completed
                    ? EStudyProgress.Studied
                    : rootNode.StudyStartedOn.HasValue
                        ? EStudyProgress.InProgress
                        : EStudyProgress.NotStarted;
            if (request.ClearPending)
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
                rootNode.StudiedOn = null;
            }

            rootNode.LastUpdateDate = DateTimeOffset.UtcNow;

            foreach (var descendant in descendants)
            {
                descendant.Progress = request.ClearPending
                    ? EStudyProgress.NotStarted
                    : request.Completed
                        ? EStudyProgress.Studied
                        : descendant.StudyStartedOn.HasValue
                            ? EStudyProgress.InProgress
                            : EStudyProgress.NotStarted;
                if (request.ClearPending)
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
            }

            if (!await journeyRepository.UpdateNodeAsync(rootNode))
            {
                Notification.CreateNotification(
                    StudyRoutineTrace.CompleteBlock,
                    "Nao foi possivel atualizar o progresso do topico.");
                return null;
            }
        }

        block.CompletedMinutes = request.ClearPending
            ? 0
            : Math.Max(0, request.CompletedMinutes);
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
        }

        if (block.Type == EStudyBlockType.Study &&
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