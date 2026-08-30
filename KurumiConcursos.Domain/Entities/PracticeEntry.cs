using KurumiConcursos.Domain.Entities.Base;

namespace KurumiConcursos.Domain.Entities;

public sealed class PracticeEntry : EntityBase
{
    public Guid UserId { get; set; }
    public long JourneyId { get; set; }
    public long? KnowledgeAreaId { get; set; }
    public long? SyllabusNodeId { get; set; }
    public DateOnly PracticeDate { get; set; }
    public int QuestionsAnswered { get; set; }
    public int CorrectAnswers { get; set; }
}