using KurumiConcursos.ApplicationService.DataTransferObjects.JourneyDtos.Response;
using KurumiConcursos.ApplicationService.Interfaces.MapperContracts;
using KurumiConcursos.ApplicationService.Interfaces.ServiceContracts;
using KurumiConcursos.Domain.ValueObjects;
using KurumiConcursos.Infra.Interfaces.RepositoryContracts;

namespace KurumiConcursos.ApplicationService.Services.JourneyServices;

public sealed class JourneyQueryService(IJourneyRepository repository, IJourneyMapper mapper) : IJourneyQueryService
{
    public async Task<IList<JourneySummaryResponse>> FindAllAsync(UserCredential userCredential) =>
        mapper.DomainToDtoSummaryResponseList(
            await repository.FindAllByAccountAsync(userCredential.UserId, CancellationToken.None));

    public async Task<JourneyDetailsResponse?> FindByIdAsync(long id, UserCredential userCredential)
    {
        var journey = await repository.FindByIdAsync(id, userCredential.UserId, CancellationToken.None, true);
        return journey is null ? null : mapper.DomainToDtoDetailsResponse(journey);
    }
}