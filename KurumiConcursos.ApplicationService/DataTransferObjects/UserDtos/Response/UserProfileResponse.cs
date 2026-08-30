using KurumiConcursos.ApplicationService.DataTransferObjects.PersonalDataDtos.Response;
using KurumiConcursos.Domain.Enums;

namespace KurumiConcursos.ApplicationService.DataTransferObjects.UserDtos.Response;

public sealed record UserProfileResponse
{
    public Guid UserId { get; init; }
    public string Email { get; init; } = string.Empty;
    public EUserStatus Status { get; init; }
    public List<string> Roles { get; init; } = [];
    public PersonalDataResponse? PersonalData { get; init; }
}
