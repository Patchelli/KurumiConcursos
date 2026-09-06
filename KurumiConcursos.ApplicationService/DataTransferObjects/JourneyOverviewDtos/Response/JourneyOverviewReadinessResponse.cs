namespace KurumiConcursos.ApplicationService.DataTransferObjects.JourneyOverviewDtos.Response;

public sealed record JourneyOverviewReadinessResponse(
    decimal Score,
    string Level,
    decimal Coverage,
    decimal Application,
    decimal Retention,
    decimal Consistency);