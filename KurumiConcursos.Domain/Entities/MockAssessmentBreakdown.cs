using KurumiConcursos.Domain.Entities.Base;

namespace KurumiConcursos.Domain.Entities;

public sealed class MockAssessmentBreakdown : EntityBase
{
    public long MockAssessmentId { get; set; }
    public MockAssessment MockAssessment { get; set; } = null!;
    public long KnowledgeAreaId { get; set; }
    public int TotalQuestions { get; set; }
    public int CorrectAnswers { get; set; }
}
