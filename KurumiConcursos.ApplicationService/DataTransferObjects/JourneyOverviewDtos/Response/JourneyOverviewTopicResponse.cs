namespace KurumiConcursos.ApplicationService.DataTransferObjects.JourneyOverviewDtos.Response;

public sealed record JourneyOverviewTopicResponse(
    long Id,
    string Title,
    decimal Coverage,
    decimal? Accuracy);