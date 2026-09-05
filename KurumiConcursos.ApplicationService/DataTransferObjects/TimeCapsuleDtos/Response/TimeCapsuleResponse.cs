namespace KurumiConcursos.ApplicationService.DataTransferObjects.TimeCapsuleDtos.Response;

public sealed record TimeCapsuleResponse(
    long Id,
    long JourneyId,
    string Title,
    string Message,
    string? VideoUrl,
    string Status,
    string TriggerType,
    long? TriggerReferenceId,
    string? TriggerReferenceLabel,
    decimal? TriggerValue,
    DateTimeOffset? ScheduledAt,
    DateTimeOffset CreatedAt,
    DateTimeOffset? DeliveredAt,
    DateTimeOffset? OpenedAt);