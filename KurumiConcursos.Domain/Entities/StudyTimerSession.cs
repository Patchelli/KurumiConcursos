using KurumiConcursos.Domain.Entities.Base;
using KurumiConcursos.Domain.Enums;

namespace KurumiConcursos.Domain.Entities;

public sealed class StudyTimerSession : EntityBase
{
    public Guid UserId { get; set; }
    public long JourneyId { get; set; }
    public long KnowledgeAreaId { get; set; }
    public long? SyllabusNodeId { get; set; }
    public EStudyTimerMode Mode { get; set; } = EStudyTimerMode.Free;
    public EStudyTimerPhase Phase { get; set; } = EStudyTimerPhase.Focus;
    public bool IsRunning { get; set; }
    public int AccumulatedFocusSeconds { get; set; }
    public int CurrentPhaseSeconds { get; set; }
    public DateTimeOffset? RunningSince { get; set; }
    public int FocusMinutes { get; set; } = 25;
    public int ShortBreakMinutes { get; set; } = 5;
    public int LongBreakMinutes { get; set; } = 15;
    public int Cycles { get; set; } = 4;
    public int CurrentCycle { get; set; } = 1;
}
