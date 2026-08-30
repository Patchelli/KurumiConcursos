namespace KurumiConcursos.ApplicationService.DataTransferObjects.UserDtos.Request;

public sealed record UserChangePasswordRequest
{
    public string CurrentPassword { get; init; } = string.Empty;
    public string NewPassword { get; init; } = string.Empty;
}