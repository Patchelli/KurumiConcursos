using KurumiConcursos.ApplicationService.Mappers;
using KurumiConcursos.Domain.Entities;
using KurumiConcursos.Domain.Enums;

namespace KurumiConcursos.UnitTests.Mappers;

public sealed class StudyTimerMapperTests
{
    private readonly StudyTimerMapper _mapper = new();

    [Fact]
    public void RunningFreeTimer_AddsElapsedTimeToFocus()
    {
        var started = DateTimeOffset.Parse("2026-09-05T12:00:00Z");
        var session = Session(EStudyTimerMode.Free, EStudyTimerPhase.Focus, started, 90, 90);

        var result = _mapper.DomainToDtoResponse(session, started.AddSeconds(30));

        Assert.True(result.IsRunning);
        Assert.Equal(120, result.AccumulatedFocusSeconds);
        Assert.Equal(120, result.CurrentPhaseSeconds);
    }

    [Fact]
    public void PomodoroFocus_StopsAtConfiguredLimit()
    {
        var started = DateTimeOffset.Parse("2026-09-05T12:00:00Z");
        var session = Session(EStudyTimerMode.Pomodoro, EStudyTimerPhase.Focus, started, 0, 0);
        session.FocusMinutes = 25;

        var result = _mapper.DomainToDtoResponse(session, started.AddMinutes(40));

        Assert.False(result.IsRunning);
        Assert.True(result.PhaseCompleted);
        Assert.Equal(1500, result.AccumulatedFocusSeconds);
        Assert.Equal(1500, result.CurrentPhaseSeconds);
    }

    [Fact]
    public void PomodoroBreak_DoesNotAddStudyTime()
    {
        var started = DateTimeOffset.Parse("2026-09-05T12:00:00Z");
        var session = Session(EStudyTimerMode.Pomodoro, EStudyTimerPhase.ShortBreak, started, 1500, 0);

        var result = _mapper.DomainToDtoResponse(session, started.AddMinutes(2));

        Assert.Equal(1500, result.AccumulatedFocusSeconds);
        Assert.Equal(120, result.CurrentPhaseSeconds);
    }

    private static StudyTimerSession Session(EStudyTimerMode mode, EStudyTimerPhase phase,
        DateTimeOffset started, int focusSeconds, int phaseSeconds) => new()
    {
        Id = 1, UserId = Guid.NewGuid(), JourneyId = 1, KnowledgeAreaId = 1,
        Mode = mode, Phase = phase, IsRunning = true, RunningSince = started,
        AccumulatedFocusSeconds = focusSeconds, CurrentPhaseSeconds = phaseSeconds,
        FocusMinutes = 25, ShortBreakMinutes = 5, LongBreakMinutes = 15, Cycles = 4, CurrentCycle = 1
    };
}
