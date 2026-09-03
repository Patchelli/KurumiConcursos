using KurumiConcursos.ApplicationService.DataTransferObjects.StudyRoutineBlockDtos.Response;
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
        // Cada materia conserva a propria fila: uma pendencia ocupa a proxima
        // ocorrencia daquela materia e desloca os topicos seguintes em cascata.
        var overdue = await studyRoutineBlockRepository.FindAllAsync(item =>
            item.StudyRoutineId == routineId &&
            item.UserId == credential.UserId &&
            item.ScheduledFor < from &&
            item.Status == EStudyBlockStatus.Pending);
        if (overdue.Count > 0)
        {
            var pending = await studyRoutineBlockRepository.FindAllAsync(item =>
                item.StudyRoutineId == routineId &&
                item.UserId == credential.UserId &&
                item.Status == EStudyBlockStatus.Pending);
            await RescheduleOverdueStudiesAsync(pending, from, credential);

            // Revisoes possuem data propria e nao participam da fila de topicos.
            foreach (var review in overdue.Where(item => item.Type == EStudyBlockType.Review))
                await MoveBlockAsync(review, from);
        }

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
            var targetDates = BuildTargetDates(ordered, futureSlots, overdueCount, from);

            for (var index = 0; index < ordered.Count; index++)
                await MoveBlockAsync(ordered[index], targetDates[index]);
        }
    }

    private static List<DateOnly> BuildTargetDates(
        IReadOnlyList<StudyRoutineBlock> ordered,
        List<DateOnly> futureSlots,
        int overdueCount,
        DateOnly from)
    {
        var targetDates = new List<DateOnly>(futureSlots);
        var occurrenceDates = ordered.Select(block => block.ScheduledFor).Distinct().Order().ToList();
        var cadenceDays = occurrenceDates.Count >= 2
            ? Math.Max(1, occurrenceDates[^1].DayNumber - occurrenceDates[^2].DayNumber)
            : 1;
        var nextDate = targetDates.Count > 0 ? targetDates[^1] : from.AddDays(-cadenceDays);

        for (var index = 0; index < overdueCount; index++)
        {
            nextDate = nextDate.AddDays(cadenceDays);
            targetDates.Add(nextDate);
        }

        return targetDates;
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