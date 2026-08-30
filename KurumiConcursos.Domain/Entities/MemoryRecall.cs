using KurumiConcursos.Domain.Entities.Base;
using KurumiConcursos.Domain.Enums;

namespace KurumiConcursos.Domain.Entities;

public sealed class MemoryRecall : EntityBase
{
    public long MemoryCardId { get; set; }
    public MemoryCard Card { get; set; } = null!;
    public ERecallGrade Grade { get; set; }
    public DateTimeOffset AnsweredAt { get; set; }
    public int PreviousIntervalDays { get; set; }
    public int NewIntervalDays { get; set; }
}