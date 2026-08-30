using KurumiConcursos.Domain.Entities.Base;
using KurumiConcursos.Domain.Enums;

namespace KurumiConcursos.Domain.Entities;

public sealed class StudyResource : EntityBase
{
    public Guid UserId { get; set; }
    public long JourneyId { get; set; }
    public long? KnowledgeAreaId { get; set; }
    public long? SyllabusNodeId { get; set; }
    public string Title { get; set; } = string.Empty;
    public string Url { get; set; } = string.Empty;
    public EResourceKind Kind { get; set; }
}