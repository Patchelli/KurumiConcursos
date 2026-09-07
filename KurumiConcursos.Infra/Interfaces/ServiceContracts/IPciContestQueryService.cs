using KurumiConcursos.Domain.ValueObjects;

namespace KurumiConcursos.Infra.Interfaces.ServiceContracts;

public interface IPciContestQueryService
{
    Task<ContestFeed> GetContestsAsync(CancellationToken cancellationToken);
}