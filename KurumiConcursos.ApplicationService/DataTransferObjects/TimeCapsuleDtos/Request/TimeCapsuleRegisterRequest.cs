namespace KurumiConcursos.ApplicationService.DataTransferObjects.TimeCapsuleDtos.Request;

public sealed record TimeCapsuleRegisterRequest(
    long JourneyId,
    string Title,
    string Message,
    string? VideoUrl,
    string TriggerType,
    long? TriggerReferenceId,
    string? TriggerReferenceLabel,
    decimal? TriggerValue,
    DateTimeOffset? ScheduledAt);