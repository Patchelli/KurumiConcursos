namespace KurumiConcursos.ApplicationService.DataTransferObjects.PrivateMaterialDtos.Response;

public sealed record NextcloudEntryResponse(string Name, string Path, bool IsDirectory, string? MimeType, long? Size);
