using KurumiConcursos.ApplicationService.DataTransferObjects.StudyTimerDtos.Request;
using KurumiConcursos.ApplicationService.DataTransferObjects.StudyTimerDtos.Response;
using KurumiConcursos.Domain.ValueObjects;

namespace KurumiConcursos.ApplicationService.Interfaces.ServiceContracts;

public interface IStudyTimerCommandService
{
    Task<StudyTimerResponse?> SaveAsync(StudyTimerSaveRequest request, UserCredential credential);
    Task<bool> FinishAsync(StudyTimerFinishRequest request, UserCredential credential);
    Task<bool> DiscardAsync(UserCredential credential);
}
