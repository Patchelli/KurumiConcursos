using KurumiConcursos.ApplicationService.DataTransferObjects.JourneyDtos.Request;
using KurumiConcursos.ApplicationService.DataTransferObjects.JourneyDtos.Response;
using KurumiConcursos.Domain.ValueObjects;

namespace KurumiConcursos.ApplicationService.Interfaces.ServiceContracts;

public interface IJourneyCommandService
{
    Task<JourneyRegisterResponse?> RegisterAsync(
        SaveJourneyStructureRequest request,
        UserCredential userCredential);

    Task<bool> UpdateAsync(SaveJourneyStructureRequest request, UserCredential userCredential);

    Task<bool> DeleteRegisterAsync(long id, UserCredential userCredential);
    Task<bool> AddAreaAsync(SaveKnowledgeAreaRequest request, UserCredential userCredential);
    Task<bool> DeleteAreaAsync(long id, UserCredential userCredential);
    Task<bool> AddNodeAsync(SaveSyllabusNodeRequest request, UserCredential userCredential);
    Task<bool> DeleteNodeAsync(long id, UserCredential userCredential);
}
