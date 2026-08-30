using KurumiConcursos.Api.Extensions;
using KurumiConcursos.ApplicationService.DataTransferObjects.JourneyDtos.Request;
using KurumiConcursos.ApplicationService.DataTransferObjects.JourneyDtos.Response;
using KurumiConcursos.ApplicationService.Interfaces.ServiceContracts;
using KurumiConcursos.Domain.UserPolicies;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace KurumiConcursos.Api.Controllers;

[ApiController]
[Route("api/journeys")]
[Authorize(Roles = $"{Policy.Student}")]
public sealed class JourneyController(
    IJourneyCommandService journeyCommandService,
    IJourneyQueryService journeyQueryService)
    : ControllerBase
{
    [HttpPost("register")]
    [ProducesResponseType(typeof(JourneyRegisterResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public Task<JourneyRegisterResponse?> Register([FromBody] JourneyRegisterRequest request) =>
        journeyCommandService.RegisterAsync(request, User.GetUserCredential());

    [HttpPut("update")]
    [ProducesResponseType(typeof(bool), StatusCodes.Status200OK)]
    public Task<bool> Update([FromBody] JourneyUpdateRequest request) =>
        journeyCommandService.UpdateAsync(request, User.GetUserCredential());

    [HttpGet("get_by_id")]
    [ProducesResponseType(typeof(JourneyDetailsResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public Task<JourneyDetailsResponse?> GetById([FromQuery] long id) =>
        journeyQueryService.FindByIdAsync(id, User.GetUserCredential());

    [HttpGet("list")]
    [ProducesResponseType(typeof(IList<JourneySummaryResponse>), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public Task<IList<JourneySummaryResponse>> List() =>
        journeyQueryService.FindAllAsync(User.GetUserCredential());

    [HttpDelete("delete")]
    [ProducesResponseType(typeof(bool), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public Task<bool> Delete([FromQuery] long id) =>
        journeyCommandService.DeleteRegisterAsync(id, User.GetUserCredential());

    [HttpPost("areas")]
    public Task<bool> AddArea([FromBody] KnowledgeAreaRegisterRequest request) =>
        journeyCommandService.AddAreaAsync(request, User.GetUserCredential());

    [HttpDelete("areas")]
    public Task<bool> DeleteArea([FromQuery] long id) =>
        journeyCommandService.DeleteAreaAsync(id, User.GetUserCredential());

    [HttpPost("nodes")]
    public Task<bool> AddNode([FromBody] SyllabusNodeRegisterRequest request) =>
        journeyCommandService.AddNodeAsync(request, User.GetUserCredential());

    [HttpDelete("nodes")]
    public Task<bool> DeleteNode([FromQuery] long id) =>
        journeyCommandService.DeleteNodeAsync(id, User.GetUserCredential());
}