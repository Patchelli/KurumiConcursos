namespace KurumiConcursos.ApplicationService.DataTransferObjects.JourneyOverviewDtos.Response;

public sealed record JourneyOverviewWeekResponse(
    string Label,
    decimal? Accuracy,
    decimal? Retention);