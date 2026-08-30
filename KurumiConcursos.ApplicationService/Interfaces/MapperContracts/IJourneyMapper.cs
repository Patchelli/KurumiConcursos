using KurumiConcursos.ApplicationService.DataTransferObjects.JourneyDtos.Request;
using KurumiConcursos.ApplicationService.DataTransferObjects.JourneyDtos.Response;
using KurumiConcursos.Domain.Entities;

namespace KurumiConcursos.ApplicationService.Interfaces.MapperContracts;

public interface IJourneyMapper
{
    ExamJourney DtoRegisterToDomain(Guid userId, JourneyRegisterRequest dto);

    ExamJourney DtoUpdateToDomain(ExamJourney entity, JourneyUpdateRequest dto);

    JourneySummaryResponse DomainToDtoSummaryResponse(ExamJourney entity);

    JourneyDetailsResponse DomainToDtoDetailsResponse(ExamJourney entity);

    IList<JourneySummaryResponse> DomainToDtoSummaryResponseList(IList<ExamJourney> entities);
}