using KurumiConcursos.Domain.Entities.Base;
using KurumiConcursos.Domain.Enums;

namespace KurumiConcursos.Domain.Entities;

public sealed class SyllabusNode : EntityBase
{
    public long KnowledgeAreaId { get; set; }
    public KnowledgeArea KnowledgeArea { get; set; } = null!;
    public long? ParentId { get; set; }
    public SyllabusNode? Parent { get; set; }
    public string Title { get; set; } = string.Empty;
    public int Order { get; set; }
    public EStudyProgress Progress { get; set; } = EStudyProgress.NotStarted;
    public DateOnly? StudyStartedOn { get; set; }
    public DateOnly? StudiedOn { get; set; }
    public ICollection<SyllabusNode> Children { get; set; } = [];
}