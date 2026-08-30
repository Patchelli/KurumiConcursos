using KurumiConcursos.ApplicationService.DataTransferObjects.AuthenticationDtos.Request;
using KurumiConcursos.ApplicationService.DataTransferObjects.AuthenticationDtos.Response;
using KurumiConcursos.Domain.ValueObjects;

namespace KurumiConcursos.ApplicationService.Interfaces.ServiceContracts;

public interface IAuthenticationCommandService
{
    Task<AuthenticationResponse?> RegisterAsync(RegisterRequest request);
    Task<AuthenticationResponse?> CreateAccessTokenAsync(LoginRequest request);
    Task<bool> LogoutAsync(UserCredential userCredential);
}