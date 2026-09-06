using KurumiConcursos.Domain.Enums;

namespace KurumiConcursos.ApplicationService.DataTransferObjects.CalendarEventDtos.Response;

public sealed record CalendarEventResponse(
    long Id,
    DateOnly Date,
    string Title,
    ECalendarEventType Type,
    string? Note,
    bool Managed = false,
    long? JourneyId = null,
    long? KnowledgeAreaId = null,
    long? SyllabusNodeId = null);
