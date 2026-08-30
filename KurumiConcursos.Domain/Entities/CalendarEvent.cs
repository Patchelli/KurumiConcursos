using KurumiConcursos.Domain.Entities.Base;
using KurumiConcursos.Domain.Enums;

namespace KurumiConcursos.Domain.Entities;

public sealed class CalendarEvent : EntityBase
{
    public Guid UserId { get; set; }
    public DateOnly Date { get; set; }
    public string Title { get; set; } = string.Empty;
    public ECalendarEventType Type { get; set; }
    public string? Note { get; set; }
}