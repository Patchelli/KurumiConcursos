namespace KurumiConcursos.ApplicationService.DataTransferObjects.TimeCapsuleDtos.Request;

public sealed record TimeCapsuleUpdateRequest(
    string Title,
    string Message,
    string? VideoUrl,
    string TriggerType,
    long? TriggerReferenceId,
    string? TriggerReferenceLabel,
    decimal? TriggerValue,
    DateTimeOffset? ScheduledAt);