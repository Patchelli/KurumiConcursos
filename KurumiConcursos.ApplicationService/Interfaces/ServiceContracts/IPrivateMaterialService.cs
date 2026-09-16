using KurumiConcursos.ApplicationService.DataTransferObjects.PrivateMaterialDtos.Request;
using KurumiConcursos.ApplicationService.DataTransferObjects.PrivateMaterialDtos.Response;
using KurumiConcursos.ApplicationService.Services.PrivateMaterialServices;
using KurumiConcursos.Domain.ValueObjects;

namespace KurumiConcursos.ApplicationService.Interfaces.ServiceContracts;

public interface IPrivateMaterialService
{
    Task<IList<NextcloudEntryResponse>> BrowseAsync(string? path, UserCredential credential,
        CancellationToken cancellationToken);

    Task<IList<PrivateMaterialResponse>> FindAllAsync(long topicId, UserCredential credential);

    Task<PrivateMaterialResponse?> LinkAsync(long topicId, PrivateMaterialLinkRequest request,
        UserCredential credential, CancellationToken cancellationToken);

    Task<bool> DeleteAsync(long topicId, long materialId, UserCredential credential);

    Task<PrivateMaterialFile?> OpenAsync(long topicId, long materialId, UserCredential credential,
        CancellationToken cancellationToken);

    Task<PrivateMaterialResponse?> UpdateStudyLocationAsync(long topicId, long materialId, string? studyLocation,
        UserCredential credential);

    Task<IList<PrivateMaterialResponse>> FindAllByAreaAsync(long areaId, UserCredential credential);

    Task<PrivateMaterialResponse?> LinkToAreaAsync(long areaId, PrivateMaterialLinkRequest request,
        UserCredential credential, CancellationToken cancellationToken);

    Task<bool> DeleteByAreaAsync(long areaId, long materialId, UserCredential credential);

    Task<PrivateMaterialFile?> OpenByAreaAsync(long areaId, long materialId, UserCredential credential,
        CancellationToken cancellationToken);

    Task<PrivateMaterialResponse?> UpdateAreaStudyLocationAsync(long areaId, long materialId, string? studyLocation,
        UserCredential credential);
}