using KurumiConcursos.Api.Extensions;
using KurumiConcursos.ApplicationService.DataTransferObjects.StudyTimerDtos.Request;
using KurumiConcursos.ApplicationService.DataTransferObjects.StudyTimerDtos.Response;
using KurumiConcursos.ApplicationService.Interfaces.ServiceContracts;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace KurumiConcursos.Api.Controllers;

[ApiController, Route("api/[controller]"), Authorize]
public sealed class StudyTimerController(IStudyTimerCommandService command, IStudyTimerQueryService query) : ControllerBase
{
    [HttpGet("active")]
    public Task<StudyTimerResponse?> Active() => query.FindActiveAsync(User.GetUserCredential());

    [HttpPut("active")]
    public Task<StudyTimerResponse?> Save(StudyTimerSaveRequest request) =>
        command.SaveAsync(request, User.GetUserCredential());

    [HttpPost("finish")]
    public Task<bool> Finish(StudyTimerFinishRequest request) => command.FinishAsync(request, User.GetUserCredential());

    [HttpDelete("active")]
    public Task<bool> Discard() => command.DiscardAsync(User.GetUserCredential());
}
