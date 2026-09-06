namespace KurumiConcursos.ApplicationService.DataTransferObjects.JourneyOverviewDtos.Response;

public sealed record JourneyOverviewErrorResponse(string Reason, int Count, decimal Percentage);