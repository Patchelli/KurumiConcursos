using KurumiConcursos.Domain.Entities;

namespace KurumiConcursos.ApplicationService.Interfaces.ServiceContracts;

public interface IQuestionAppointmentCommandService
{
    Task<bool> ScheduleAsync(Guid userId, long journeyId, SyllabusNode node, DateOnly completedOn,
        DateOnly? reviewDate = null);
    Task<bool> SupersedePendingAsync(Guid userId, IReadOnlyCollection<long> syllabusNodeIds);
    Task<bool> CompletePendingAsync(Guid userId, long journeyId, long syllabusNodeId);
}
