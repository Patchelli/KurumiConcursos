using KurumiConcursos.Domain.Enums;

namespace KurumiConcursos.ApplicationService.DataTransferObjects.CalendarEventDtos.Request;

public sealed record CalendarEventRegisterRequest(DateOnly Date, string Title, ECalendarEventType Type, string? Note);