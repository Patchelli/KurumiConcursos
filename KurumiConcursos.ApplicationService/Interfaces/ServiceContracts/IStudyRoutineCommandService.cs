using KurumiConcursos.ApplicationService.DataTransferObjects.StudyRoutineBlockDtos.Request;
using KurumiConcursos.ApplicationService.DataTransferObjects.StudyRoutineBlockDtos.Response;
using KurumiConcursos.ApplicationService.DataTransferObjects.StudyRoutineDtos.Request;
using KurumiConcursos.ApplicationService.DataTransferObjects.StudyRoutineDtos.Response;
using KurumiConcursos.Domain.ValueObjects;

namespace KurumiConcursos.ApplicationService.Interfaces.ServiceContracts;

public interface IStudyRoutineCommandService
{
    Task<StudyRoutineResponse?> RegisterAsync(
        StudyRoutineRegisterRequest request,
        UserCredential credential);

    Task<StudyRoutineResponse?> UpdateAsync(
        StudyRoutineUpdateRequest request,
        UserCredential credential);

    Task<IList<StudyRoutineBlockResponse>> GenerateAsync(
        StudyRoutineGenerateRequest request,
        UserCredential credential);

    Task<StudyRoutineBlockResponse?> CompleteBlockAsync(
        StudyRoutineBlockCompleteRequest request,
        UserCredential credential);
}