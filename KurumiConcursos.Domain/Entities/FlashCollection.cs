using KurumiConcursos.Domain.Entities.Base;

namespace KurumiConcursos.Domain.Entities;

public sealed class FlashCollection : EntityBase
{
    public Guid UserId { get; set; }
    public long JourneyId { get; set; }
    public long? ParentId { get; set; }
    public string Title { get; set; } = string.Empty;
    public int Order { get; set; }
    public ICollection<MemoryCard> Cards { get; set; } = [];
}