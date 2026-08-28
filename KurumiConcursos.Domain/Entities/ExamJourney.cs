using KurumiConcursos.Domain.Entities.Base;
using KurumiConcursos.Domain.Enums;

namespace KurumiConcursos.Domain.Entities;

public sealed class ExamJourney : EntityBase
{
    public Guid UserId { get; set; }
    public StudentProfile StudentProfile { get; set; } = null!;
    public string Title { get; set; } = string.Empty;
    public string? Institution { get; set; }
    public string? ExamBoard { get; set; }
    public string? Position { get; set; }
    public decimal? Salary { get; set; }
    public int? Openings { get; set; }
    public string? NoticeUrl { get; set; }
    public DateOnly? ExamDate { get; set; }
    public EJourneyStage Stage { get; set; } = EJourneyStage.PreNotice;
    public bool IncludeInStatistics { get; set; } = true;
    public int CompletedSyllabusCycles { get; set; }
    public string? LogoUrl { get; set; }
    public ICollection<KnowledgeArea> KnowledgeAreas { get; set; } = [];
}
