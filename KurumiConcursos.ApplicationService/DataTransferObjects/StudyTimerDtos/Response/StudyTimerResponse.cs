using KurumiConcursos.Domain.Enums;

namespace KurumiConcursos.ApplicationService.DataTransferObjects.StudyTimerDtos.Response;

public sealed record StudyTimerResponse(
    long Id,
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
    int CurrentCycle,
    DateTimeOffset ServerNow,
    bool PhaseCompleted);
