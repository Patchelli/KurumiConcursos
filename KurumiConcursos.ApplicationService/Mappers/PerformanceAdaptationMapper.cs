using KurumiConcursos.ApplicationService.Interfaces.MapperContracts;
using KurumiConcursos.Domain.Entities;
using KurumiConcursos.Domain.Enums;

namespace KurumiConcursos.ApplicationService.Mappers;

public sealed class PerformanceAdaptationMapper : IPerformanceAdaptationMapper
{
    public ReviewAppointment DomainToReviewAppointment(Guid userId, long syllabusNodeId, DateOnly date,
        EAdaptationTrigger trigger) => new()
    {
        UserId = userId,
        SyllabusNodeId = syllabusNodeId,
        ScheduledFor = date,
        AdaptationTrigger = trigger
    };

    public QuestionAppointment DomainToQuestionAppointment(Guid userId, long journeyId, long syllabusNodeId,
        DateOnly date, EAdaptationTrigger trigger) => new()
    {
        UserId = userId,
        JourneyId = journeyId,
        SyllabusNodeId = syllabusNodeId,
        ScheduledFor = date,
        AdaptationTrigger = trigger
    };
}