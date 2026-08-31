using KurumiConcursos.Domain.Entities.Base;
using KurumiConcursos.Domain.Enums;

namespace KurumiConcursos.Domain.Entities;

public sealed class StudyRoutineBlock : EntityBase
{
    public Guid UserId { get; set; }
    public long JourneyId { get; set; }
    public long StudyRoutineId { get; set; }
    public long SyllabusNodeId { get; set; }
    public DateOnly ScheduledFor { get; set; }
    public EStudyBlockType Type { get; set; }
    public EStudyBlockStatus Status { get; set; }
    public int PlannedMinutes { get; set; }
    public int CompletedMinutes { get; set; }
    public int Order { get; set; }
    public DateTimeOffset? CompletedAt { get; set; }
}