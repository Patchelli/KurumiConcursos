namespace KurumiConcursos.ApplicationService.DataTransferObjects.JourneyOverviewDtos.Response;

public sealed record JourneyOverviewSummaryResponse(
    int StudiedMinutes,
    int Questions,
    int CorrectAnswers,
    decimal? Accuracy,
    int StudyDays,
    int CompletedTopics,
    int TotalTopics,
    int CompletedSubtopics,
    int TotalSubtopics,
    int TodayMinutes,
    int TodayQuestions,
    decimal? TodayAccuracy,
    int TodaySessions,
    int StudyStreak);
