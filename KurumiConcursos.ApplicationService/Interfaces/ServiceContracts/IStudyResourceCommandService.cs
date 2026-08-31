using KurumiConcursos.ApplicationService.DataTransferObjects.StudyResourceDtos.Request;
using KurumiConcursos.ApplicationService.DataTransferObjects.StudyResourceDtos.Response;
using KurumiConcursos.Domain.ValueObjects;

namespace KurumiConcursos.ApplicationService.Interfaces.ServiceContracts;

public interface IStudyResourceCommandService
{
    Task<StudyResourceResponse?> RegisterAsync(
        StudyResourceRegisterRequest request,
        UserCredential credential);

    Task<bool> DeleteAsync(long id, UserCredential credential);
}