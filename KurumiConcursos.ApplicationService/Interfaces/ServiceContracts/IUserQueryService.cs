using KurumiConcursos.ApplicationService.DataTransferObjects.UserDtos.Response;
using KurumiConcursos.Domain.ValueObjects;

namespace KurumiConcursos.ApplicationService.Interfaces.ServiceContracts;

public interface IUserQueryService
{
    Task<UserProfileResponse?> GetMyProfileAsync(UserCredential credential);
}
