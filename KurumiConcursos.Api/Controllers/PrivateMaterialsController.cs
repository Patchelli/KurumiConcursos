using KurumiConcursos.Api.Extensions;
using KurumiConcursos.ApplicationService.DataTransferObjects.PrivateMaterialDtos.Request;
using KurumiConcursos.ApplicationService.DataTransferObjects.PrivateMaterialDtos.Response;
using KurumiConcursos.ApplicationService.Interfaces.ServiceContracts;
using KurumiConcursos.Domain.UserPolicies;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace KurumiConcursos.Api.Controllers;

[ApiController, Route("api"), Authorize(Policy = PrivateMaterials.PolicyName)]
public sealed class PrivateMaterialsController(IPrivateMaterialService service) : ControllerBase
{
    [HttpGet("private-materials/files")]
    public Task<IList<NextcloudEntryResponse>> Browse([FromQuery] string? path, CancellationToken cancellationToken) =>
        service.BrowseAsync(path, User.GetUserCredential(), cancellationToken);

    [HttpGet("topics/{topicId:long}/materials")]
    public Task<IList<PrivateMaterialResponse>> List(long topicId) => service.FindAllAsync(topicId, User.GetUserCredential());

    [HttpPost("topics/{topicId:long}/materials")]
    public Task<PrivateMaterialResponse?> Link(long topicId, PrivateMaterialLinkRequest request, CancellationToken cancellationToken) =>
        service.LinkAsync(topicId, request, User.GetUserCredential(), cancellationToken);

    [HttpDelete("topics/{topicId:long}/materials/{materialId:long}")]
    public Task<bool> Delete(long topicId, long materialId) => service.DeleteAsync(topicId, materialId, User.GetUserCredential());

    [HttpGet("topics/{topicId:long}/materials/{materialId:long}/file")]
    public async Task<IActionResult> File(long topicId, long materialId, CancellationToken cancellationToken)
    {
        var file = await service.OpenAsync(topicId, materialId, User.GetUserCredential(), cancellationToken);
        if (file is null) return NotFound();
        HttpContext.Response.RegisterForDispose(file.Disposable);
        return base.File(file.Stream, "application/pdf", enableRangeProcessing: false);
    }

    [HttpGet("knowledge-areas/{areaId:long}/materials")]
    public Task<IList<PrivateMaterialResponse>> ListArea(long areaId) => service.FindAllByAreaAsync(areaId, User.GetUserCredential());

    [HttpPost("knowledge-areas/{areaId:long}/materials")]
    public Task<PrivateMaterialResponse?> LinkArea(long areaId, PrivateMaterialLinkRequest request, CancellationToken cancellationToken) =>
        service.LinkToAreaAsync(areaId, request, User.GetUserCredential(), cancellationToken);

    [HttpDelete("knowledge-areas/{areaId:long}/materials/{materialId:long}")]
    public Task<bool> DeleteArea(long areaId, long materialId) => service.DeleteByAreaAsync(areaId, materialId, User.GetUserCredential());

    [HttpGet("knowledge-areas/{areaId:long}/materials/{materialId:long}/file")]
    public async Task<IActionResult> AreaFile(long areaId, long materialId, CancellationToken cancellationToken)
    {
        var file = await service.OpenByAreaAsync(areaId, materialId, User.GetUserCredential(), cancellationToken);
        if (file is null) return NotFound();
        HttpContext.Response.RegisterForDispose(file.Disposable);
        return base.File(file.Stream, "application/pdf", enableRangeProcessing: false);
    }
}
