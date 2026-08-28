using KurumiConcursos.ApplicationService.DataTransferObjects.JourneyDtos.Response;
using KurumiConcursos.Domain.ValueObjects;

namespace KurumiConcursos.ApplicationService.Interfaces.ServiceContracts;

public interface IJourneyQueryService
{
    Task<IList<JourneySummaryResponse>> FindAllAsync(UserCredential userCredential);

    Task<JourneyDetailsResponse?> FindByIdAsync(long id, UserCredential userCredential);
}
