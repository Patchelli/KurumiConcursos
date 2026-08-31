using KurumiConcursos.ApplicationService.DataTransferObjects.StudyRoutineBlockDtos.Response;
using KurumiConcursos.ApplicationService.DataTransferObjects.StudyRoutineDtos.Response;
using KurumiConcursos.Domain.ValueObjects;

namespace KurumiConcursos.ApplicationService.Interfaces.ServiceContracts;

public interface IStudyRoutineQueryService
{
    Task<IList<StudyRoutineResponse>> FindAllAsync(
        long journeyId,
        UserCredential credential);

    Task<IList<StudyRoutineBlockResponse>> FindBlocksAsync(
        long routineId,
        DateOnly from,
        DateOnly to,
        UserCredential credential);
}