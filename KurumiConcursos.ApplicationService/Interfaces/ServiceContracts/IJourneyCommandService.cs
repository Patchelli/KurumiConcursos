using KurumiConcursos.ApplicationService.DataTransferObjects.JourneyDtos.Request;
using KurumiConcursos.ApplicationService.DataTransferObjects.JourneyDtos.Response;
using KurumiConcursos.Domain.ValueObjects;

namespace KurumiConcursos.ApplicationService.Interfaces.ServiceContracts;

public interface IJourneyCommandService
{
    Task<JourneyRegisterResponse?> RegisterAsync(
        JourneyRegisterRequest request,
        UserCredential userCredential);

    Task<bool> UpdateAsync(JourneyUpdateRequest request, UserCredential userCredential);

    Task<bool> DeleteRegisterAsync(long id, UserCredential userCredential);
    Task<bool> AddAreaAsync(KnowledgeAreaRegisterRequest request, UserCredential userCredential);
    Task<bool> DeleteAreaAsync(long id, UserCredential userCredential);
    Task<bool> AddNodeAsync(SyllabusNodeRegisterRequest request, UserCredential userCredential);
    Task<bool> DeleteNodeAsync(long id, UserCredential userCredential);
}