using KurumiConcursos.Api.Extensions;
using KurumiConcursos.ApplicationService.DataTransferObjects.AuthenticationDtos.Request;
using KurumiConcursos.ApplicationService.DataTransferObjects.AuthenticationDtos.Response;
using KurumiConcursos.ApplicationService.Interfaces.ServiceContracts;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace KurumiConcursos.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public sealed class AuthenticationController(
    IAuthenticationCommandService authenticationCommandService)
    : ControllerBase
{
    [AllowAnonymous]
    [HttpPost("register")]
    [ProducesResponseType(typeof(AuthenticationResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public Task<AuthenticationResponse?> Register([FromBody] RegisterRequest request) =>
        authenticationCommandService.RegisterAsync(request);

    [AllowAnonymous]
    [HttpPost("generate_access_token")]
    [ProducesResponseType(typeof(AuthenticationResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public Task<AuthenticationResponse?> GenerateAccessToken([FromBody] LoginRequest request) =>
        authenticationCommandService.CreateAccessTokenAsync(request);

    [HttpPost("logout")]
    [ProducesResponseType(typeof(bool), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public Task<bool> Logout() =>
        authenticationCommandService.LogoutAsync(User.GetUserCredential());
}