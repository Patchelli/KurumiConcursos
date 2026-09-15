namespace KurumiConcursos.ApplicationService.DataTransferObjects.PrivateMaterialDtos.Response;

public sealed record PrivateMaterialResponse(long Id, long TopicId, string Name, string MimeType, DateTimeOffset CreatedAt);
