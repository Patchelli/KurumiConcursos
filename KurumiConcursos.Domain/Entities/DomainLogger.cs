using KurumiConcursos.Domain.Enums;

namespace KurumiConcursos.Domain.Entities;

public sealed class DomainLogger
{
    public long Id { get; init; }
    public EUserAction Action { get; init; }
    public DateTimeOffset ActionDate { get; init; }
    public required string Description { get; init; }
    public Guid UserId { get; init; }
    public string? EntityId { get; set; }
}
