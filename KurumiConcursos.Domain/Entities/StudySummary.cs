using KurumiConcursos.Domain.Entities.Base;

namespace KurumiConcursos.Domain.Entities;

public sealed class StudySummary : EntityBase
{
    public Guid UserId { get; set; }
    public long JourneyId { get; set; }
    public long SyllabusNodeId { get; set; }
    public long? ReviewAppointmentId { get; set; }
    public bool IsReview { get; set; }
    public string Content { get; set; } = string.Empty;
}