using KurumiConcursos.Domain.Entities.Base;

namespace KurumiConcursos.Domain.Entities;

public sealed class KnowledgeArea : EntityBase
{
    public long JourneyId { get; set; }
    public ExamJourney Journey { get; set; } = null!;
    public string Title { get; set; } = string.Empty;
    public int Order { get; set; }
    public decimal? Weight { get; set; }
    public int? ExpectedQuestions { get; set; }
    public ICollection<SyllabusNode> SyllabusNodes { get; set; } = [];
}
