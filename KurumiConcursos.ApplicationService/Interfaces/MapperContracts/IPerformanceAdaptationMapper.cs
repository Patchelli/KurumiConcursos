using KurumiConcursos.Domain.Entities;
using KurumiConcursos.Domain.Enums;

namespace KurumiConcursos.ApplicationService.Interfaces.MapperContracts;

public interface IPerformanceAdaptationMapper
{
    ReviewAppointment DomainToReviewAppointment(Guid userId, long syllabusNodeId, DateOnly date,
        EAdaptationTrigger trigger);
    QuestionAppointment DomainToQuestionAppointment(Guid userId, long journeyId, long syllabusNodeId,
        DateOnly date, EAdaptationTrigger trigger);
}
