using KurumiConcursos.ApplicationService.DataTransferObjects.PrivateMaterialDtos.Response;

namespace KurumiConcursos.ApplicationService.Services.PrivateMaterialServices;

public interface INextcloudWebDavService
{
    Task<IList<NextcloudEntryResponse>> BrowseAsync(string? path, CancellationToken cancellationToken);
    Task<NextcloudFileMetadata?> FindPdfAsync(string path, CancellationToken cancellationToken);
    Task<PrivateMaterialFile?> OpenPdfAsync(string path, CancellationToken cancellationToken);
}

public sealed record NextcloudFileMetadata(string Name, string Path, string MimeType);
