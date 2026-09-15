using KurumiConcursos.ApplicationService.DataTransferObjects.PrivateMaterialDtos.Request;
using KurumiConcursos.ApplicationService.DataTransferObjects.PrivateMaterialDtos.Response;
using KurumiConcursos.ApplicationService.Interfaces.ServiceContracts;
using KurumiConcursos.Domain.Entities;
using KurumiConcursos.Domain.Interface;
using KurumiConcursos.Domain.ValueObjects;
using KurumiConcursos.Infra.Interfaces.RepositoryContracts;

namespace KurumiConcursos.ApplicationService.Services.PrivateMaterialServices;

public sealed class PrivateMaterialService(
    ITopicMaterialRepository materials,
    IJourneyRepository journeys,
    INextcloudWebDavService nextcloud,
    IValidate<TopicMaterial> validation,
    INotificationHandler notification) : IPrivateMaterialService
{
    public Task<IList<NextcloudEntryResponse>> BrowseAsync(string? path, UserCredential credential,
        CancellationToken cancellationToken) => nextcloud.BrowseAsync(path, cancellationToken);

    public async Task<IList<PrivateMaterialResponse>> FindAllAsync(long topicId, UserCredential credential) =>
        (await materials.FindAllAsync(topicId, credential.UserId)).Select(Map).ToList();

    public async Task<PrivateMaterialResponse?> LinkAsync(long topicId, PrivateMaterialLinkRequest request,
        UserCredential credential, CancellationToken cancellationToken)
    {
        var topic = await journeys.FindNodeAsync(topicId, credential.UserId, cancellationToken);
        if (topic is null)
        {
            notification.CreateNotification("Materiais privados", "Topico nao encontrado.");
            return null;
        }

        var remote = await nextcloud.FindPdfAsync(request.NextcloudPath, cancellationToken);
        if (remote is null)
        {
            notification.CreateNotification("Materiais privados", "O arquivo PDF nao foi encontrado no Nextcloud.");
            return null;
        }

        var material = new TopicMaterial { SyllabusNodeId = topic.Id, Name = remote.Name, NextcloudPath = remote.Path, MimeType = remote.MimeType };
        if (!validation.Validation(material).Valid || !await materials.SaveAsync(material))
        {
            notification.CreateNotification("Materiais privados", "Nao foi possivel vincular o material.");
            return null;
        }
        return Map(material);
    }

    public async Task<bool> DeleteAsync(long topicId, long materialId, UserCredential credential)
    {
        var material = await materials.FindAsync(materialId, topicId, credential.UserId, tracking: true);
        if (material is null)
            return notification.CreateNotification("Materiais privados", "Material nao encontrado.");
        return await materials.DeleteAsync(material);
    }

    public async Task<PrivateMaterialFile?> OpenAsync(long topicId, long materialId, UserCredential credential,
        CancellationToken cancellationToken)
    {
        var material = await materials.FindAsync(materialId, topicId, credential.UserId);
        return material is null ? null : await nextcloud.OpenPdfAsync(material.NextcloudPath, cancellationToken);
    }

    public async Task<IList<PrivateMaterialResponse>> FindAllByAreaAsync(long areaId, UserCredential credential) =>
        (await materials.FindAllByAreaAsync(areaId, credential.UserId)).Select(Map).ToList();

    public async Task<PrivateMaterialResponse?> LinkToAreaAsync(long areaId, PrivateMaterialLinkRequest request,
        UserCredential credential, CancellationToken cancellationToken)
    {
        var area = await journeys.FindAreaAsync(areaId, credential.UserId, cancellationToken);
        if (area is null) return null;
        var remote = await nextcloud.FindPdfAsync(request.NextcloudPath, cancellationToken);
        if (remote is null) return null;
        var material = new TopicMaterial { KnowledgeAreaId = area.Id, Name = remote.Name, NextcloudPath = remote.Path, MimeType = remote.MimeType };
        return !validation.Validation(material).Valid || !await materials.SaveAsync(material) ? null : Map(material);
    }

    public async Task<bool> DeleteByAreaAsync(long areaId, long materialId, UserCredential credential)
    {
        var material = await materials.FindByAreaAsync(materialId, areaId, credential.UserId, true);
        return material is not null && await materials.DeleteAsync(material);
    }

    public async Task<PrivateMaterialFile?> OpenByAreaAsync(long areaId, long materialId, UserCredential credential,
        CancellationToken cancellationToken)
    {
        var material = await materials.FindByAreaAsync(materialId, areaId, credential.UserId);
        return material is null ? null : await nextcloud.OpenPdfAsync(material.NextcloudPath, cancellationToken);
    }

    private static PrivateMaterialResponse Map(TopicMaterial material) =>
        new(material.Id, material.SyllabusNodeId ?? material.KnowledgeAreaId!.Value, material.Name, material.MimeType, material.CreationDate);
}
