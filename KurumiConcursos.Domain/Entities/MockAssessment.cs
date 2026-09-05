using KurumiConcursos.Domain.Entities.Base;

namespace KurumiConcursos.Domain.Entities;

public sealed class MockAssessment : EntityBase
{
    public Guid UserId { get; set; }
    public long JourneyId { get; set; }
    public string Title { get; set; } = string.Empty;
    public string? Source { get; set; }
    public DateOnly AssessmentDate { get; set; }
    public int DurationMinutes { get; set; }
    public int TotalQuestions { get; set; }
    public int CorrectAnswers { get; set; }
    public decimal? Score { get; set; }
    public ICollection<MockAssessmentBreakdown> Breakdown { get; set; } = [];
}