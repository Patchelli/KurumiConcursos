namespace KurumiConcursos.ApplicationService.DataTransferObjects.JourneyDtos.Request;

public sealed record SyllabusNodeStudyRequest(
    long JourneyId,
    long SyllabusNodeId,
    bool Completed,
    int StudiedMinutes,
    bool ScheduleReview,
    DateOnly? ReviewDate,
    bool ClearPending = false,
    int? StudiedSeconds = null);
