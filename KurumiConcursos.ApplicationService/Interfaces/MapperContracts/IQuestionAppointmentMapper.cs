using KurumiConcursos.Domain.Entities;

namespace KurumiConcursos.ApplicationService.Interfaces.MapperContracts;

public interface IQuestionAppointmentMapper
{
    QuestionAppointment DomainToAppointment(Guid userId, long journeyId, SyllabusNode node, DateOnly date);
}
