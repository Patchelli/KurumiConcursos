using KurumiConcursos.Domain.ValueObjects;

namespace KurumiConcursos.Infra.Interfaces.MapperContracts;

public interface IPciContestMapper
{
    IReadOnlyList<ContestOpportunity> ResponseToDomain(string response);
}