using KurumiConcursos.Api.Extensions;
using KurumiConcursos.ApplicationService.DataTransferObjects.RadarDtos.Request;
using KurumiConcursos.ApplicationService.DataTransferObjects.RadarDtos.Response;
using KurumiConcursos.ApplicationService.Interfaces.ServiceContracts;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace KurumiConcursos.Api.Controllers;

[ApiController]
[Authorize]
[Route("api/radar")]
public sealed class RadarController(IRadarCommandService commandService, IRadarQueryService queryService)
    : ControllerBase
{
    [HttpGet("preferences")]
    [ProducesResponseType(typeof(RadarPreferencesResponse), StatusCodes.Status200OK)]
    public Task<RadarPreferencesResponse?> Preferences() => queryService.GetPreferencesAsync(User.GetUserCredential());

    [HttpPut("preferences")]
    [ProducesResponseType(typeof(bool), StatusCodes.Status200OK)]
    public Task<bool> Save(RadarPreferencesRequest request) =>
        commandService.SavePreferencesAsync(request, User.GetUserCredential());

    [HttpGet("contests")]
    [ProducesResponseType(typeof(RadarResultResponse), StatusCodes.Status200OK)]
    public Task<RadarResultResponse?> Contests(CancellationToken cancellationToken) =>
        queryService.GetContestsAsync(User.GetUserCredential(), cancellationToken);
}