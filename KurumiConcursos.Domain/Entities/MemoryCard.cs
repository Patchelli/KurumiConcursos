using KurumiConcursos.Domain.Entities.Base;

namespace KurumiConcursos.Domain.Entities;

public sealed class MemoryCard : EntityBase
{
    public long FlashCollectionId { get; set; }
    public FlashCollection Collection { get; set; } = null!;
    public string Front { get; set; } = string.Empty;
    public string Back { get; set; } = string.Empty;
    public string Model { get; set; } = string.Empty;
    public string Type { get; set; } = string.Empty;
    public bool? CorrectAnswer { get; set; }
    public DateOnly? NextReviewOn { get; set; }
    public int IntervalDays { get; set; }
    public decimal EaseFactor { get; set; } = 2.5m;
    public ICollection<MemoryRecall> Recalls { get; set; } = [];
}