using KurumiConcursos.Api.Extensions;
using KurumiConcursos.ApplicationService.DataTransferObjects.JourneyOverviewDtos.Response;
using KurumiConcursos.ApplicationService.Interfaces.ServiceContracts;
using KurumiConcursos.Domain.UserPolicies;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace KurumiConcursos.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize(Roles = $"{Policy.Student}")]
public sealed class JourneyOverviewController(IJourneyOverviewQueryService queryService) : ControllerBase
{
    [HttpGet("get")]
    [ProducesResponseType(typeof(JourneyOverviewResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public Task<JourneyOverviewResponse?> Get([FromQuery] long journeyId) =>
        queryService.FindAsync(journeyId, User.GetUserCredential());
}