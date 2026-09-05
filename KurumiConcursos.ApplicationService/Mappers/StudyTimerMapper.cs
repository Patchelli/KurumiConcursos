using KurumiConcursos.ApplicationService.DataTransferObjects.StudyTimerDtos.Request;
using KurumiConcursos.ApplicationService.DataTransferObjects.StudyTimerDtos.Response;
using KurumiConcursos.ApplicationService.Interfaces.MapperContracts;
using KurumiConcursos.Domain.Entities;
using KurumiConcursos.Domain.Enums;

namespace KurumiConcursos.ApplicationService.Mappers;

public sealed class StudyTimerMapper : IStudyTimerMapper
{
    public StudyTimerSession DtoSaveToDomain(Guid userId, StudyTimerSaveRequest request, DateTimeOffset now)
    {
        var session = new StudyTimerSession { UserId = userId };
        DtoSaveToDomain(request, session, now);
        return session;
    }

    public void DtoSaveToDomain(StudyTimerSaveRequest r, StudyTimerSession s, DateTimeOffset now)
    {
        s.JourneyId = r.JourneyId; s.KnowledgeAreaId = r.KnowledgeAreaId; s.SyllabusNodeId = r.SyllabusNodeId;
        s.Mode = r.Mode; s.Phase = r.Phase; s.IsRunning = r.IsRunning;
        s.AccumulatedFocusSeconds = Math.Max(0, r.AccumulatedFocusSeconds);
        s.CurrentPhaseSeconds = Math.Max(0, r.CurrentPhaseSeconds);
        s.FocusMinutes = r.FocusMinutes; s.ShortBreakMinutes = r.ShortBreakMinutes; s.LongBreakMinutes = r.LongBreakMinutes;
        s.Cycles = r.Cycles; s.CurrentCycle = r.CurrentCycle;
        s.RunningSince = r.IsRunning ? now : null; s.LastUpdateDate = now;
    }

    public StudyTimerResponse DomainToDtoResponse(StudyTimerSession s, DateTimeOffset now)
    {
        var elapsed = s.IsRunning && s.RunningSince.HasValue ? Math.Max(0, (int)(now - s.RunningSince.Value).TotalSeconds) : 0;
        var limit = s.Mode == EStudyTimerMode.Pomodoro ? (s.Phase switch
        {
            EStudyTimerPhase.Focus => s.FocusMinutes,
            EStudyTimerPhase.ShortBreak => s.ShortBreakMinutes,
            _ => s.LongBreakMinutes
        }) * 60 : int.MaxValue;
        var phaseSeconds = Math.Min(limit, s.CurrentPhaseSeconds + elapsed);
        var addedFocus = s.Phase == EStudyTimerPhase.Focus ? phaseSeconds - s.CurrentPhaseSeconds : 0;
        var completed = s.Mode == EStudyTimerMode.Pomodoro && phaseSeconds >= limit;
        return new(s.Id, s.JourneyId, s.KnowledgeAreaId, s.SyllabusNodeId, s.Mode, s.Phase,
            s.IsRunning && !completed, s.AccumulatedFocusSeconds + addedFocus, phaseSeconds,
            s.FocusMinutes, s.ShortBreakMinutes, s.LongBreakMinutes, s.Cycles, s.CurrentCycle, now, completed);
    }
}
