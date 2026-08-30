using KurumiConcursos.Api.Extensions;
using KurumiConcursos.ApplicationService.DataTransferObjects.PersonalDataDtos.Request;
using KurumiConcursos.ApplicationService.DataTransferObjects.UserDtos.Request;
using KurumiConcursos.ApplicationService.DataTransferObjects.UserDtos.Response;
using KurumiConcursos.ApplicationService.Interfaces.ServiceContracts;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace KurumiConcursos.Api.Controllers;

[ApiController]
[Route("api/user")]
[Authorize]
public sealed class UserController(
    IUserQueryService userQueryService,
    IUserCommandService userCommandService) : ControllerBase
{
    [HttpGet("me")]
    [ProducesResponseType(typeof(UserProfileResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public Task<UserProfileResponse?> Me() =>
        userQueryService.GetMyProfileAsync(User.GetUserCredential());

    [HttpPut("me/personal-data")]
    [ProducesResponseType(typeof(bool), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public Task<bool> UpdatePersonalData([FromBody] PersonalDataUpdateRequest request) =>
        userCommandService.UpdateMyPersonalDataAsync(request, User.GetUserCredential());

    [HttpPost("change_password")]
    [ProducesResponseType(typeof(bool), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public Task<bool> ChangePassword([FromBody] UserChangePasswordRequest request) =>
        userCommandService.ChangePasswordAsync(request, User.GetUserCredential());
}