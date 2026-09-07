using KurumiConcursos.ApplicationService.DataTransferObjects.CalendarEventDtos.Response;
using KurumiConcursos.ApplicationService.Interfaces.MapperContracts;
using KurumiConcursos.ApplicationService.Interfaces.ServiceContracts;
using KurumiConcursos.Domain.ValueObjects;
using KurumiConcursos.Infra.Interfaces.RepositoryContracts;
using Microsoft.EntityFrameworkCore;

namespace KurumiConcursos.ApplicationService.Services.CalendarServices;

public sealed class CalendarEventQueryService(
    ICalendarEventRepository calendarEventRepository,
    IQuestionAppointmentRepository questionAppointmentRepository,
    ICalendarEventMapper calendarEventMapper)
    : ICalendarEventQueryService
{
    public async Task<IList<CalendarEventResponse>> FindAllAsync(UserCredential credential)
    {
        var calendarEvents = await calendarEventRepository.FindAllAsync(item => item.UserId == credential.UserId);
        var questionAppointments = await questionAppointmentRepository.FindAllAsync(item =>
                item.UserId == credential.UserId && !item.Completed && !item.Superseded,
            query => query.Include(item => item.SyllabusNode).ThenInclude(item => item.KnowledgeArea));

        return calendarEventMapper.DomainToDtoResponseList(calendarEvents)
            .Concat(questionAppointments.Select(calendarEventMapper.DomainToDtoResponse))
            .OrderBy(item => item.Date)
            .ThenBy(item => item.Id)
            .ToList();
    }
}