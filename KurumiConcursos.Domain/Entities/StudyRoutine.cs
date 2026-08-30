using KurumiConcursos.Domain.Entities.Base;
using KurumiConcursos.Domain.Enums;

namespace KurumiConcursos.Domain.Entities;

public sealed class StudyRoutine : EntityBase
{
    public Guid UserId { get; set; }
    public long JourneyId { get; set; }
    public string Title { get; set; } = string.Empty;
    public ERoutineKind Kind { get; set; }
    public bool Active { get; set; }
    public string ConfigurationJson { get; set; } = "{}";
}