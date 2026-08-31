namespace KurumiConcursos.ApplicationService.DataTransferObjects.StudyRoutineBlockDtos.Request;

public sealed record StudyRoutineBlockCompleteRequest(
    long BlockId,
    bool Completed,
    int CompletedMinutes,
    bool ScheduleReview,
    DateOnly? ReviewDate,
    bool ClearPending = false);