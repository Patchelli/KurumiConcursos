using KurumiConcursos.ApplicationService.DataTransferObjects.CalendarEventDtos.Request;
using KurumiConcursos.ApplicationService.DataTransferObjects.CalendarEventDtos.Response;
using KurumiConcursos.Domain.ValueObjects;

namespace KurumiConcursos.ApplicationService.Interfaces.ServiceContracts;

public interface ICalendarEventCommandService
{
    Task<CalendarEventResponse?> RegisterAsync(CalendarEventRegisterRequest request, UserCredential credential);
    Task<bool> UpdateAsync(CalendarEventUpdateRequest request, UserCredential credential);
    Task<bool> DeleteAsync(long id, UserCredential credential);
}