using KurumiConcursos.ApplicationService.DataTransferObjects.JourneyOverviewDtos.Response;

namespace KurumiConcursos.ApplicationService.Interfaces.MapperContracts;

public interface IJourneyOverviewMapper
{
    JourneyOverviewResponse DomainToDtoResponse(
        JourneyOverviewSummaryResponse summary,
        JourneyOverviewReadinessResponse readiness,
        IList<JourneyOverviewDayResponse> days,
        IList<JourneyOverviewWeekResponse> weeks,
        IList<JourneyOverviewAreaResponse> areas,
        IList<JourneyOverviewErrorResponse> errors);
}