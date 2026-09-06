using KurumiConcursos.ApplicationService.Interfaces.MapperContracts;
using KurumiConcursos.Domain.Entities;

namespace KurumiConcursos.ApplicationService.Mappers;

public sealed class QuestionAppointmentMapper : IQuestionAppointmentMapper
{
    public QuestionAppointment DomainToAppointment(Guid userId, long journeyId, SyllabusNode node, DateOnly date) =>
        new()
        {
            UserId = userId,
            JourneyId = journeyId,
            SyllabusNodeId = node.Id,
            ScheduledFor = date,
            Completed = false,
            Superseded = false
        };
}
