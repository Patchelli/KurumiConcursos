using KurumiConcursos.Domain.Entities.Base;

namespace KurumiConcursos.Domain.Entities;

public sealed class PersonalData : EntityBase
{
    public Guid UserId { get; set; }
    public User User { get; set; } = null!;

    public string? FullName { get; set; }
    public string? Document { get; set; }
    public string? Phone { get; set; }
    public int? Age { get; set; }
    public DateTimeOffset? DateOfBirth { get; set; }
}