using KurumiConcursos.Domain.Entities.Base;
using KurumiConcursos.Domain.Enums;

namespace KurumiConcursos.Domain.Entities;

public sealed class TimeCapsule : EntityBase
{
    public Guid UserId { get; set; }
    public long JourneyId { get; set; }
    public string Title { get; set; } = string.Empty;
    public string Message { get; set; } = string.Empty;
    public string? VideoUrl { get; set; }
    public ECapsuleStatus Status { get; set; } = ECapsuleStatus.Scheduled;
    public ECapsuleTriggerType TriggerType { get; set; }
    public long? TriggerReferenceId { get; set; }
    public string? TriggerReferenceLabel { get; set; }
    public decimal? TriggerValue { get; set; }
    public DateTimeOffset? ScheduledAt { get; set; }
    public DateTimeOffset? DeliveredAt { get; set; }
    public DateTimeOffset? OpenedAt { get; set; }
}