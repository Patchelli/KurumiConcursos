namespace KurumiConcursos.Domain.ValueObjects;

public sealed record ContestFeed(IReadOnlyList<ContestOpportunity> Contests, DateTimeOffset UpdatedAt);