namespace KurumiConcursos.ApplicationService.DataTransferObjects.JourneyOverviewDtos.Response;

public sealed record JourneyOverviewResponse(
    JourneyOverviewSummaryResponse Summary,
    JourneyOverviewReadinessResponse Readiness,
    IList<JourneyOverviewDayResponse> Days,
    IList<JourneyOverviewWeekResponse> Weeks,
    IList<JourneyOverviewAreaResponse> Areas,
    IList<JourneyOverviewErrorResponse> Errors);