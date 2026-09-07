using KurumiConcursos.ApplicationService.DataTransferObjects.JourneyDtos.Response;
using KurumiConcursos.ApplicationService.Interfaces.MapperContracts;
using KurumiConcursos.ApplicationService.Interfaces.ServiceContracts;
using KurumiConcursos.Domain.ValueObjects;
using KurumiConcursos.Infra.Interfaces.RepositoryContracts;

namespace KurumiConcursos.ApplicationService.Services.JourneyServices;

public sealed class JourneyQueryService(
    IJourneyRepository repository,
    IJourneyOverviewQueryService overviewQueryService,
    IJourneyMapper mapper) : IJourneyQueryService
{
    public async Task<IList<JourneySummaryResponse>> FindAllAsync(UserCredential userCredential)
    {
        var journeys = await repository.FindAllByAccountAsync(userCredential.UserId, CancellationToken.None);
        var response = new List<JourneySummaryResponse>(journeys.Count);
        foreach (var journey in journeys)
        {
            var overview = await overviewQueryService.FindAsync(journey.Id, userCredential);
            if (overview is not null)
                response.Add(mapper.DomainToDtoSummaryResponse(journey, overview));
        }

        return response;
    }

    public async Task<JourneyDetailsResponse?> FindByIdAsync(long id, UserCredential userCredential)
    {
        var journey = await repository.FindByIdAsync(id, userCredential.UserId, CancellationToken.None, true);
        return journey is null ? null : mapper.DomainToDtoDetailsResponse(journey);
    }
}