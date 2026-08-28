namespace KurumiConcursos.Domain.ValueObjects;

public sealed record UserCredential
{
    public Guid UserId { get; init; }
    public required List<string> Roles { get; set; }
}
