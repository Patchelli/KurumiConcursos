using KurumiConcursos.ApplicationService.DataTransferObjects.JourneyOverviewDtos.Response;
using KurumiConcursos.Domain.ValueObjects;

namespace KurumiConcursos.ApplicationService.Interfaces.ServiceContracts;

public interface IJourneyOverviewQueryService
{
    Task<JourneyOverviewResponse?> FindAsync(long journeyId, UserCredential credential);
}