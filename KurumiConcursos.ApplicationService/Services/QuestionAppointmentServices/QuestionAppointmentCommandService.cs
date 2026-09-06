using KurumiConcursos.ApplicationService.Interfaces.MapperContracts;
using KurumiConcursos.ApplicationService.Interfaces.ServiceContracts;
using KurumiConcursos.Domain.Entities;
using KurumiConcursos.Infra.Interfaces.RepositoryContracts;

namespace KurumiConcursos.ApplicationService.Services.QuestionAppointmentServices;

public sealed class QuestionAppointmentCommandService(
    IQuestionAppointmentRepository repository,
    IQuestionAppointmentMapper mapper) : IQuestionAppointmentCommandService
{
    public Task<bool> ScheduleAsync(Guid userId, long journeyId, SyllabusNode node, DateOnly completedOn) =>
        repository.SaveAsync(mapper.DomainToAppointment(
            userId, journeyId, node, completedOn.AddDays(Random.Shared.Next(1, 3))));

    public async Task<bool> SupersedePendingAsync(Guid userId, IReadOnlyCollection<long> syllabusNodeIds)
    {
        var appointments = await repository.FindAllAsync(item => item.UserId == userId &&
            syllabusNodeIds.Contains(item.SyllabusNodeId) && !item.Completed && !item.Superseded);
        foreach (var appointment in appointments)
        {
            appointment.Superseded = true;
            appointment.LastUpdateDate = DateTimeOffset.UtcNow;
            if (!await repository.UpdateAsync(appointment)) return false;
        }

        return true;
    }

    public async Task<bool> CompletePendingAsync(Guid userId, long journeyId, long syllabusNodeId)
    {
        var appointments = await repository.FindAllAsync(item => item.UserId == userId &&
            item.JourneyId == journeyId && item.SyllabusNodeId == syllabusNodeId &&
            !item.Completed && !item.Superseded);
        foreach (var appointment in appointments)
        {
            appointment.Completed = true;
            appointment.CompletedAt = DateTimeOffset.UtcNow;
            appointment.LastUpdateDate = DateTimeOffset.UtcNow;
            if (!await repository.UpdateAsync(appointment)) return false;
        }

        return true;
    }
}
