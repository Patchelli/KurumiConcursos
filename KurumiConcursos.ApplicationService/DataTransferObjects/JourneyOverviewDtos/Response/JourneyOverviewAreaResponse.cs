namespace KurumiConcursos.ApplicationService.DataTransferObjects.JourneyOverviewDtos.Response;

public sealed record JourneyOverviewAreaResponse(
    long Id,
    string Title,
    int StudiedMinutes,
    decimal Coverage,
    int Questions,
    int CorrectAnswers,
    decimal? Accuracy,
    int Errors,
    int ErrorsWithReason,
    string? PredominantError,
    int PredominantErrorCount,
    decimal? PredominantErrorPercentage,
    int Flashcards,
    int Reviews,
    decimal? Recall,
    IList<JourneyOverviewTopicResponse> Topics);