using KurumiConcursos.Domain.Entities.Base;

namespace KurumiConcursos.Domain.Entities;

public sealed class FocusSession : EntityBase
{
    public Guid AccountId { get; set; }
    public long JourneyId { get; set; }
    public long? KnowledgeAreaId { get; set; }
    public long? SyllabusNodeId { get; set; }
    public DateOnly StudyDate { get; set; }
    public int DurationSeconds { get; set; }
    public string? Notes { get; set; }
}
