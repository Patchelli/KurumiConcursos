using KurumiConcursos.ApplicationService.DataTransferObjects.StudyTimerDtos.Response;
using KurumiConcursos.Domain.ValueObjects;

namespace KurumiConcursos.ApplicationService.Interfaces.ServiceContracts;

public interface IStudyTimerQueryService
{
    Task<StudyTimerResponse?> FindActiveAsync(UserCredential credential);
}
