using KurumiConcursos.ApplicationService.DataTransferObjects.PersonalDataDtos.Request;

namespace KurumiConcursos.ApplicationService.DataTransferObjects.AuthenticationDtos.Request;

public sealed record RegisterRequest
{
    public string Email { get; init; } = string.Empty;
    public string Password { get; init; } = string.Empty;
    public PersonalDataRegisterRequest PersonalData { get; init; } = new();
}