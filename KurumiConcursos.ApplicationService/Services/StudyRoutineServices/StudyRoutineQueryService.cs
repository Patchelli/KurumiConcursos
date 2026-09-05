using System.Text.Json;
using KurumiConcursos.ApplicationService.DataTransferObjects.StudyRoutineBlockDtos.Response;
using KurumiConcursos.ApplicationService.DataTransferObjects.StudyRoutineDtos.Request;
using KurumiConcursos.ApplicationService.DataTransferObjects.StudyRoutineDtos.Response;
using KurumiConcursos.ApplicationService.Interfaces.MapperContracts;
using KurumiConcursos.ApplicationService.Interfaces.ServiceContracts;
using KurumiConcursos.Domain.Entities;
using KurumiConcursos.Domain.Enums;
using KurumiConcursos.Domain.ValueObjects;
using KurumiConcursos.Infra.Interfaces.RepositoryContracts;

namespace KurumiConcursos.ApplicationService.Services.StudyRoutineServices;

public sealed class StudyRoutineQueryService(
    IStudyRoutineRepository studyRoutineRepository,
    IStudyRoutineBlockRepository studyRoutineBlockRepository,
    IJourneyRepository journeyRepository,
    IStudyRoutineMapper studyRoutineMapper)
    : IStudyRoutineQueryService
{
    private static readonly JsonSerializerOptions JsonOptions = new()
    {
        PropertyNameCaseInsensitive = true
    };

    public async Task<IList<StudyRoutineResponse>> FindAllAsync(
        long journeyId,
        UserCredential credential)
    {
        var routines = await studyRoutineRepository.FindAllAsync(item =>
            item.UserId == credential.UserId &&
            (journeyId <= 0 || item.JourneyId == journeyId));

        return routines.Select(studyRoutineMapper.DomainToDtoResponse).ToList();
    }

    public async Task<IList<StudyRoutineBlockResponse>> FindBlocksAsync(
        long routineId,
        DateOnly from,
        DateOnly to,
        UserCredential credential)
    {
        var scheduleFrom = CurrentDate();
        // Cada materia conserva a propria fila: uma pendencia ocupa a proxima
        // ocorrencia daquela materia e desloca os topicos seguintes em cascata.
        var overdue = await studyRoutineBlockRepository.FindAllAsync(item =>
            item.StudyRoutineId == routineId &&
            item.UserId == credential.UserId &&
            item.ScheduledFor < scheduleFrom &&
            item.Status == EStudyBlockStatus.Pending);
        StudyRoutineConfigurationRequest? configuration = null;
        IList<StudyRoutineBlock>? pending = null;
        if (overdue.Count > 0)
        {
            var routine = await studyRoutineRepository.FindByPredicateAsync(item =>
                item.Id == routineId && item.UserId == credential.UserId,
                asNoTracking: true);
            pending = await studyRoutineBlockRepository.FindAllAsync(item =>
                item.StudyRoutineId == routineId &&
                item.UserId == credential.UserId &&
                item.Status == EStudyBlockStatus.Pending);
            configuration = routine is null
                ? null
                : JsonSerializer.Deserialize<StudyRoutineConfigurationRequest>(
                    routine.ConfigurationJson,
                    JsonOptions);
            await RescheduleOverdueStudiesAsync(pending, scheduleFrom, configuration, credential);

            // Revisoes possuem data propria e nao participam da fila de topicos.
            foreach (var review in overdue.Where(item => item.Type == EStudyBlockType.Review))
                await MoveBlockAsync(review, scheduleFrom);
        }

        // Tambem corrige planos que tenham sido abertos antes desta regra e ja
        // estejam com blocos acumulados em uma mesma data.
        if (pending is null)
        {
            var routine = await studyRoutineRepository.FindByPredicateAsync(item =>
                item.Id == routineId && item.UserId == credential.UserId,
                asNoTracking: true);
            configuration = routine is null
                ? null
                : JsonSerializer.Deserialize<StudyRoutineConfigurationRequest>(
                    routine.ConfigurationJson,
                    JsonOptions);
            pending = await studyRoutineBlockRepository.FindAllAsync(item =>
                item.StudyRoutineId == routineId &&
                item.UserId == credential.UserId &&
                item.Status == EStudyBlockStatus.Pending);
        }
        if (configuration is not null)
            await EnforceDailyCapacityAsync(pending, scheduleFrom, configuration);

        var allBlocks = await studyRoutineBlockRepository.FindAllAsync(item =>
            item.StudyRoutineId == routineId &&
            item.UserId == credential.UserId &&
            item.ScheduledFor >= from &&
            item.ScheduledFor <= to);
        var completedStudyNodes = (await studyRoutineBlockRepository.FindAllAsync(item =>
                item.StudyRoutineId == routineId &&
                item.UserId == credential.UserId &&
                item.Type == EStudyBlockType.Study &&
                item.Status == EStudyBlockStatus.Completed))
            .Select(item => item.SyllabusNodeId)
            .ToHashSet();

        return allBlocks
            .Where(item => item.Type != EStudyBlockType.Review || completedStudyNodes.Contains(item.SyllabusNodeId))
            .GroupBy(item => new { item.ScheduledFor, item.SyllabusNodeId, item.Type })
            .Select(group => group
                .OrderByDescending(item => item.Status == EStudyBlockStatus.Completed)
                .ThenBy(item => item.Order)
                .First())
            .Select(ToResponse)
            .ToList();
    }

    private async Task RescheduleOverdueStudiesAsync(
        IList<StudyRoutineBlock> pending,
        DateOnly from,
        StudyRoutineConfigurationRequest? configuration,
        UserCredential credential)
    {
        var studies = pending.Where(item => item.Type == EStudyBlockType.Study).ToList();
        if (!studies.Any(item => item.ScheduledFor < from))
            return;

        var journeyId = studies[0].JourneyId;
        var journey = await journeyRepository.FindByIdAsync(
            journeyId,
            credential.UserId,
            CancellationToken.None,
            includeStructure: true);
        if (journey is null)
            return;

        var areaByNode = journey.KnowledgeAreas
            .SelectMany(area => area.SyllabusNodes.Select(node => new { node.Id, AreaId = area.Id }))
            .ToDictionary(item => item.Id, item => item.AreaId);

        foreach (var subjectQueue in studies
                     .Where(block => areaByNode.ContainsKey(block.SyllabusNodeId))
                     .GroupBy(block => areaByNode[block.SyllabusNodeId])
                     .Where(group => group.Any(block => block.ScheduledFor < from)))
        {
            var ordered = subjectQueue
                .OrderBy(block => block.ScheduledFor)
                .ThenBy(block => block.Order)
                .ToList();
            var overdueCount = ordered.Count(block => block.ScheduledFor < from);
            var futureSlots = ordered
                .Where(block => block.ScheduledFor >= from)
                .Select(block => block.ScheduledFor)
                .ToList();
            var targetDates = BuildTargetDates(
                ordered,
                futureSlots,
                overdueCount,
                from,
                EstimateSubjectCadence(configuration));

            for (var index = 0; index < ordered.Count; index++)
                await MoveBlockAsync(ordered[index], targetDates[index]);
        }
    }

    private static List<DateOnly> BuildTargetDates(
        IReadOnlyList<StudyRoutineBlock> ordered,
        List<DateOnly> futureSlots,
        int overdueCount,
        DateOnly from,
        int defaultCadenceDays)
    {
        var targetDates = new List<DateOnly>(futureSlots);
        var occurrenceDates = ordered.Select(block => block.ScheduledFor).Distinct().Order().ToList();
        var cadenceDays = occurrenceDates.Count >= 2
            ? Math.Max(1, occurrenceDates[^1].DayNumber - occurrenceDates[^2].DayNumber)
            : defaultCadenceDays;
        var nextDate = targetDates.Count > 0
            ? targetDates[^1]
            : occurrenceDates.Count > 0
                ? occurrenceDates[^1]
                : from;

        for (var index = 0; index < overdueCount; index++)
        {
            do
                nextDate = nextDate.AddDays(cadenceDays);
            while (nextDate < from);
            targetDates.Add(nextDate);
        }

        return targetDates;
    }

    private async Task EnforceDailyCapacityAsync(
        IList<StudyRoutineBlock> pending,
        DateOnly from,
        StudyRoutineConfigurationRequest configuration)
    {
        var studies = pending
            .Where(item => item.Type == EStudyBlockType.Study && item.ScheduledFor >= from)
            .ToList();
        if (studies.Count == 0)
            return;

        // Um reagendamento nunca cria uma quarta vaga em um dia configurado para tres.
        // Quando a data esta cheia, o ultimo bloco segue para o dia seguinte e provoca
        // o mesmo deslocamento, preservando a capacidade original em efeito domino.
        for (var date = from; date < from.AddDays(365); date = date.AddDays(1))
        {
            var capacity = AvailableMinutes(configuration, date);
            var blocks = studies
                .Where(item => item.ScheduledFor == date)
                .OrderBy(item => item.Order)
                .ToList();
            if (capacity <= 0)
            {
                var target = NextAvailableStudyDay(configuration, date.AddDays(1));
                foreach (var block in blocks)
                    await MoveBlockAsync(block, target);
                continue;
            }

            var used = blocks.Sum(item => item.PlannedMinutes);
            while (used > capacity && blocks.Count > 0)
            {
                var block = blocks[^1];
                blocks.RemoveAt(blocks.Count - 1);
                used -= block.PlannedMinutes;

                var target = NextAvailableStudyDay(configuration, date.AddDays(1));
                await MoveBlockAsync(block, target);
            }
        }
    }

    private static int EstimateSubjectCadence(StudyRoutineConfigurationRequest? configuration)
    {
        if (configuration is null || configuration.KnowledgeAreaIds.Count == 0)
            return 1;

        var dailyTopics = configuration.Availability.Values
            .Where(hours => hours > 0)
            .Select(hours => Math.Max(1, (int)Math.Floor(hours / Math.Max(configuration.HoursPerTopic, 0.01m))))
            .DefaultIfEmpty(1)
            .Average();
        return Math.Max(1, (int)Math.Ceiling(configuration.KnowledgeAreaIds.Count / dailyTopics));
    }

    private static DateOnly NextAvailableStudyDay(
        StudyRoutineConfigurationRequest configuration,
        DateOnly date)
    {
        while (AvailableMinutes(configuration, date) <= 0)
            date = date.AddDays(1);
        return date;
    }

    private static int AvailableMinutes(
        StudyRoutineConfigurationRequest configuration,
        DateOnly date)
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
        return (int)Math.Max(
            0,
            Math.Round((double)configuration.Availability.GetValueOrDefault(key)) * 60);
    }

    private static DateOnly CurrentDate()
    {
        var zone = TimeZoneInfo.FindSystemTimeZoneById(
            OperatingSystem.IsWindows() ? "E. South America Standard Time" : "America/Sao_Paulo");
        return DateOnly.FromDateTime(TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow, zone));
    }

    private async Task MoveBlockAsync(StudyRoutineBlock block, DateOnly targetDate)
    {
        if (block.ScheduledFor == targetDate)
            return;

        block.ScheduledFor = targetDate;
        block.LastUpdateDate = DateTimeOffset.UtcNow;
        await studyRoutineBlockRepository.UpdateAsync(block);
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
