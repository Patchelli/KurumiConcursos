using KurumiConcursos.Api.Extensions;
using KurumiConcursos.ApplicationService.DataTransferObjects.TimeCapsuleDtos.Request;
using KurumiConcursos.ApplicationService.DataTransferObjects.TimeCapsuleDtos.Response;
using KurumiConcursos.ApplicationService.Interfaces.ServiceContracts;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace KurumiConcursos.Api.Controllers;

[ApiController, Route("api/[controller]"), Authorize]
public sealed class TimeCapsuleController(ITimeCapsuleCommandService command, ITimeCapsuleQueryService query)
    : ControllerBase
{
    [HttpGet("delivered")]
    public Task<IList<TimeCapsuleResponse>> FindDelivered() =>
        query.FindDeliveredAsync(User.GetUserCredential());

    [HttpGet("list")]
    public Task<IList<TimeCapsuleResponse>> FindAll([FromQuery] long journeyId) =>
        query.FindAllAsync(journeyId, User.GetUserCredential());

    [HttpPost("register")]
    public Task<TimeCapsuleResponse?> Register(TimeCapsuleRegisterRequest request) =>
        command.RegisterAsync(request, User.GetUserCredential());

    [HttpPut("{id:long}")]
    public Task<TimeCapsuleResponse?> Update(long id, TimeCapsuleUpdateRequest request) =>
        command.UpdateAsync(id, request, User.GetUserCredential());

    [HttpPut("{id:long}/open")]
    public Task<TimeCapsuleResponse?> Open(long id) => command.OpenAsync(id, User.GetUserCredential());

    [HttpDelete("{id:long}")]
    public Task<bool> Delete(long id) => command.DeleteAsync(id, User.GetUserCredential());
}
