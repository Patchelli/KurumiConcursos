using KurumiConcursos.ApplicationService.DataTransferObjects.JourneyOverviewDtos.Response;
using KurumiConcursos.ApplicationService.Interfaces.MapperContracts;

namespace KurumiConcursos.ApplicationService.Mappers;

public sealed class JourneyOverviewMapper : IJourneyOverviewMapper
{
    public JourneyOverviewResponse DomainToDtoResponse(JourneyOverviewSummaryResponse summary,
        JourneyOverviewReadinessResponse readiness, IList<JourneyOverviewDayResponse> days,
        IList<JourneyOverviewWeekResponse> weeks, IList<JourneyOverviewAreaResponse> areas,
        IList<JourneyOverviewErrorResponse> errors) => new(summary, readiness, days, weeks, areas, errors);
}