using KurumiConcursos.ApplicationService.DataTransferObjects.PersonalDataDtos.Request;
using KurumiConcursos.ApplicationService.DataTransferObjects.UserDtos.Request;
using KurumiConcursos.Domain.ValueObjects;

namespace KurumiConcursos.ApplicationService.Interfaces.ServiceContracts;

public interface IUserCommandService
{
    Task<bool> UpdateMyPersonalDataAsync(PersonalDataUpdateRequest request, UserCredential credential);
    Task<bool> ChangePasswordAsync(UserChangePasswordRequest request, UserCredential credential);
}