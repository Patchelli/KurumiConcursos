using KurumiConcursos.Domain.Entities.Base;

namespace KurumiConcursos.Domain.Entities;

public sealed class ReviewAppointment : EntityBase
{
    public Guid UserId { get; set; }
    public long SyllabusNodeId { get; set; }
    public DateOnly ScheduledFor { get; set; }
    public bool Completed { get; set; }
    public DateTimeOffset? CompletedAt { get; set; }
    public bool Superseded { get; set; }
}