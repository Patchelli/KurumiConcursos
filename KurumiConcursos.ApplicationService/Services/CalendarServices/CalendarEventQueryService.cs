using KurumiConcursos.ApplicationService.DataTransferObjects.CalendarEventDtos.Response;
using KurumiConcursos.ApplicationService.Interfaces.MapperContracts;
using KurumiConcursos.ApplicationService.Interfaces.ServiceContracts;
using KurumiConcursos.Domain.ValueObjects;
using KurumiConcursos.Infra.Interfaces.RepositoryContracts;

namespace KurumiConcursos.ApplicationService.Services.CalendarServices;

public sealed class CalendarEventQueryService(
    ICalendarEventRepository calendarEventRepository,
    ICalendarEventMapper calendarEventMapper)
    : ICalendarEventQueryService
{
    public async Task<IList<CalendarEventResponse>> FindAllAsync(UserCredential credential)
    {
        var calendarEvents = await calendarEventRepository.FindAllAsync(item => item.UserId == credential.UserId);

        return calendarEventMapper.DomainToDtoResponseList(calendarEvents);
    }
}