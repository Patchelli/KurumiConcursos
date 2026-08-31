using KurumiConcursos.Api.Extensions;
using KurumiConcursos.ApplicationService.DataTransferObjects.StudyResourceDtos.Request;
using KurumiConcursos.ApplicationService.DataTransferObjects.StudyResourceDtos.Response;
using KurumiConcursos.ApplicationService.Interfaces.ServiceContracts;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace KurumiConcursos.Api.Controllers;

[ApiController, Route("api/[controller]"), Authorize]
public sealed class StudyResourceController(
    IStudyResourceCommandService command,
    IStudyResourceQueryService query) : ControllerBase
{
    [HttpGet("list")]
    public Task<IList<StudyResourceResponse>> List([FromQuery] long journeyId, [FromQuery] long? syllabusNodeId) =>
        query.FindAllAsync(journeyId, syllabusNodeId, User.GetUserCredential());

    [HttpPost("register")]
    public Task<StudyResourceResponse?> Register(StudyResourceRegisterRequest request) =>
        command.RegisterAsync(request, User.GetUserCredential());

    [HttpDelete("{id:long}")]
    public Task<bool> Delete(long id) => command.DeleteAsync(id, User.GetUserCredential());
}