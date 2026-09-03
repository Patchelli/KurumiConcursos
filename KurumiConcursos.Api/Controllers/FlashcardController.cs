using KurumiConcursos.Api.Extensions;
using KurumiConcursos.ApplicationService.DataTransferObjects.FlashcardDtos.Request;
using KurumiConcursos.ApplicationService.DataTransferObjects.FlashcardDtos.Response;
using KurumiConcursos.ApplicationService.Interfaces.ServiceContracts;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace KurumiConcursos.Api.Controllers;

[ApiController, Route("api/[controller]"), Authorize]
public sealed class FlashcardController(IFlashcardCommandService command, IFlashcardQueryService query) : ControllerBase
{
    [HttpPost("register")]
    public Task<FlashcardResponse?> Register(FlashcardRegisterRequest request) =>
        command.RegisterAsync(request, User.GetUserCredential());

    [HttpGet("practice")]
    public Task<FlashcardPracticeResponse> Practice(
        [FromQuery] long journeyId, [FromQuery] long? knowledgeAreaId,
        [FromQuery] long? syllabusNodeId, [FromQuery] bool includeDescendants = false) =>
        query.FindPracticeAsync(journeyId, knowledgeAreaId, syllabusNodeId, includeDescendants,
            User.GetUserCredential());

    [HttpPost("recall")]
    public Task<FlashcardResponse?> Recall(FlashcardRecallRequest request) =>
        command.RecallAsync(request, User.GetUserCredential());

    [HttpGet("list")]
    public Task<IList<FlashcardResponse>> List(
        [FromQuery] long journeyId, [FromQuery] long? knowledgeAreaId, [FromQuery] long? syllabusNodeId) =>
        query.FindAllAsync(journeyId, knowledgeAreaId, syllabusNodeId, User.GetUserCredential());

    [HttpPut("update")]
    public Task<FlashcardResponse?> Update(FlashcardUpdateRequest request) =>
        command.UpdateAsync(request, User.GetUserCredential());

    [HttpDelete("{id:long}")]
    public Task<bool> Delete(long id) => command.DeleteAsync(id, User.GetUserCredential());
}