using KurumiConcursos.Api.Extensions;
using KurumiConcursos.ApplicationService.DataTransferObjects.CalendarEventDtos.Request;
using KurumiConcursos.ApplicationService.DataTransferObjects.CalendarEventDtos.Response;
using KurumiConcursos.ApplicationService.Interfaces.ServiceContracts;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace KurumiConcursos.Api.Controllers;

[ApiController, Route("api/[controller]"), Authorize]
public sealed class CalendarController(ICalendarEventCommandService command, ICalendarEventQueryService query)
    : ControllerBase
{
    [HttpGet("list")]
    public Task<IList<CalendarEventResponse>> List() => query.FindAllAsync(User.GetUserCredential());

    [HttpPost("register")]
    public Task<CalendarEventResponse?> Register(CalendarEventRegisterRequest request) =>
        command.RegisterAsync(request, User.GetUserCredential());

    [HttpPut("update")]
    public Task<bool> Update(CalendarEventUpdateRequest request) =>
        command.UpdateAsync(request, User.GetUserCredential());

    [HttpDelete("delete")]
    public Task<bool> Delete(long id) => command.DeleteAsync(id, User.GetUserCredential());
}