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
        // Pendencias anteriores sao trazidas para o primeiro dia consultado.
        // Os blocos futuros sao deslocados para preservar a sequencia do plano.
        var overdue = await studyRoutineBlockRepository.FindAllAsync(item =>
            item.StudyRoutineId == routineId &&
            item.UserId == credential.UserId &&
            item.ScheduledFor < from &&
            item.Status == EStudyBlockStatus.Pending);
        if (overdue.Count > 0)
        {
            var future = await studyRoutineBlockRepository.FindAllAsync(item =>
                item.StudyRoutineId == routineId &&
                item.UserId == credential.UserId &&
                item.ScheduledFor >= from &&
                item.Status == EStudyBlockStatus.Pending);
            foreach (var block in future)
            {
                block.ScheduledFor = block.ScheduledFor.AddDays(1);
                await studyRoutineBlockRepository.UpdateAsync(block);
            }

            foreach (var block in overdue)
            {
                block.ScheduledFor = from;
                await studyRoutineBlockRepository.UpdateAsync(block);
            }
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