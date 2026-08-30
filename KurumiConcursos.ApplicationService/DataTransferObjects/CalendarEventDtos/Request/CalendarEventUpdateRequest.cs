using KurumiConcursos.Domain.Enums;

namespace KurumiConcursos.ApplicationService.DataTransferObjects.CalendarEventDtos.Request;

public sealed record CalendarEventUpdateRequest(
    long Id,
    DateOnly Date,
    string Title,
    ECalendarEventType Type,
    string? Note);