using KurumiConcursos.ApplicationService.DataTransferObjects.CalendarEventDtos.Response;
using KurumiConcursos.Domain.ValueObjects;

namespace KurumiConcursos.ApplicationService.Interfaces.ServiceContracts;

public interface ICalendarEventQueryService
{
    Task<IList<CalendarEventResponse>> FindAllAsync(UserCredential credential);
}