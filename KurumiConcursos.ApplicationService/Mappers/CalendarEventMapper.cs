using KurumiConcursos.ApplicationService.DataTransferObjects.CalendarEventDtos.Request;
using KurumiConcursos.ApplicationService.DataTransferObjects.CalendarEventDtos.Response;
using KurumiConcursos.ApplicationService.Interfaces.MapperContracts;
using KurumiConcursos.Domain.Entities;

namespace KurumiConcursos.ApplicationService.Mappers;

public sealed class CalendarEventMapper : ICalendarEventMapper
{
    public CalendarEvent DtoRegisterToDomain(Guid userId, CalendarEventRegisterRequest dto) => new()
        { UserId = userId, Date = dto.Date, Title = dto.Title.Trim(), Type = dto.Type, Note = dto.Note };

    public CalendarEvent DtoUpdateToDomain(CalendarEvent entity, CalendarEventUpdateRequest dto)
    {
        entity.Date = dto.Date;
        entity.Title = dto.Title.Trim();
        entity.Type = dto.Type;
        entity.Note = dto.Note;
        entity.LastUpdateDate = DateTimeOffset.UtcNow;
        return entity;
    }

    public CalendarEventResponse DomainToDtoResponse(CalendarEvent e) => new(e.Id, e.Date, e.Title, e.Type, e.Note);

    public CalendarEventResponse DomainToDtoResponse(QuestionAppointment entity) => new(
        -entity.Id,
        entity.ScheduledFor,
        $"Questões: {entity.SyllabusNode.Title}",
        Domain.Enums.ECalendarEventType.Questions,
        entity.SyllabusNode.KnowledgeArea.Title,
        true,
        entity.JourneyId,
        entity.SyllabusNode.KnowledgeAreaId,
        entity.SyllabusNodeId);

    public IList<CalendarEventResponse> DomainToDtoResponseList(IList<CalendarEvent> entities) =>
        entities.Select(DomainToDtoResponse).ToList();
}
