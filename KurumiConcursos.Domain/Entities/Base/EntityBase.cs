namespace KurumiConcursos.Domain.Entities.Base;

public abstract class EntityBase
{
    public long Id { get; set; }
    public DateTimeOffset CreationDate { get; set; } = DateTimeOffset.UtcNow;
    public DateTimeOffset? LastUpdateDate { get; set; }
}