using KurumiConcursos.ApplicationService.DataTransferObjects.JourneyDtos.Request;
using KurumiConcursos.ApplicationService.DataTransferObjects.JourneyDtos.Response;
using KurumiConcursos.ApplicationService.DataTransferObjects.JourneyOverviewDtos.Response;
using KurumiConcursos.Domain.Entities;

namespace KurumiConcursos.ApplicationService.Interfaces.MapperContracts;

public interface IJourneyMapper
{
    ExamJourney DtoRegisterToDomain(Guid userId, JourneyRegisterRequest dto);

    ExamJourney DtoUpdateToDomain(ExamJourney entity, JourneyUpdateRequest dto);

    JourneySummaryResponse DomainToDtoSummaryResponse(ExamJourney entity, JourneyOverviewResponse overview);

    JourneyDetailsResponse DomainToDtoDetailsResponse(ExamJourney entity);
}