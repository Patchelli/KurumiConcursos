using KurumiConcursos.Api.Extensions;
using KurumiConcursos.ApplicationService.DataTransferObjects.StudyRoutineBlockDtos.Request;
using KurumiConcursos.ApplicationService.DataTransferObjects.StudyRoutineBlockDtos.Response;
using KurumiConcursos.ApplicationService.DataTransferObjects.StudyRoutineDtos.Request;
using KurumiConcursos.ApplicationService.DataTransferObjects.StudyRoutineDtos.Response;
using KurumiConcursos.ApplicationService.Interfaces.ServiceContracts;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace KurumiConcursos.Api.Controllers;

[ApiController, Route("api/[controller]"), Authorize]
public sealed class StudyRoutineController(
    IStudyRoutineCommandService command,
    IStudyRoutineQueryService query) : ControllerBase
{
    [HttpGet("list")]
    public Task<IList<StudyRoutineResponse>> List([FromQuery] long journeyId) =>
        query.FindAllAsync(journeyId, User.GetUserCredential());

    [HttpPost("register")]
    public Task<StudyRoutineResponse?> Register(StudyRoutineRegisterRequest request) =>
        command.RegisterAsync(request, User.GetUserCredential());

    [HttpPut("update")]
    public Task<StudyRoutineResponse?> Update(StudyRoutineUpdateRequest request) =>
        command.UpdateAsync(request, User.GetUserCredential());

    [HttpPost("generate")]
    public Task<IList<StudyRoutineBlockResponse>> Generate(StudyRoutineGenerateRequest request) =>
        command.GenerateAsync(request, User.GetUserCredential());

    [HttpGet("blocks")]
    public Task<IList<StudyRoutineBlockResponse>> Blocks([FromQuery] long routineId, [FromQuery] DateOnly from,
        [FromQuery] DateOnly to) => query.FindBlocksAsync(routineId, from, to, User.GetUserCredential());

    [HttpPut("blocks/complete")]
    public Task<StudyRoutineBlockResponse?> CompleteBlock(StudyRoutineBlockCompleteRequest request) =>
        command.CompleteBlockAsync(request, User.GetUserCredential());
}