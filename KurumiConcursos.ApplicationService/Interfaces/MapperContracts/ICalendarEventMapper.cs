using KurumiConcursos.ApplicationService.DataTransferObjects.CalendarEventDtos.Request;
using KurumiConcursos.ApplicationService.DataTransferObjects.CalendarEventDtos.Response;
using KurumiConcursos.Domain.Entities;

namespace KurumiConcursos.ApplicationService.Interfaces.MapperContracts;

public interface ICalendarEventMapper
{
    CalendarEvent DtoRegisterToDomain(Guid userId, CalendarEventRegisterRequest dto);
    CalendarEvent DtoUpdateToDomain(CalendarEvent entity, CalendarEventUpdateRequest dto);
    CalendarEventResponse DomainToDtoResponse(CalendarEvent entity);
    IList<CalendarEventResponse> DomainToDtoResponseList(IList<CalendarEvent> entities);
}