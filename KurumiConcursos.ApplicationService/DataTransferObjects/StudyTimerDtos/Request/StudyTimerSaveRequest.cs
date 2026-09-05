using KurumiConcursos.Domain.Enums;

namespace KurumiConcursos.ApplicationService.DataTransferObjects.StudyTimerDtos.Request;

public sealed record StudyTimerSaveRequest(
    long JourneyId,
    long KnowledgeAreaId,
    long? SyllabusNodeId,
    EStudyTimerMode Mode,
    EStudyTimerPhase Phase,
    bool IsRunning,
    int AccumulatedFocusSeconds,
    int CurrentPhaseSeconds,
    int FocusMinutes,
    int ShortBreakMinutes,
    int LongBreakMinutes,
    int Cycles,
    int CurrentCycle);
