using KurumiConcursos.ApplicationService.DataTransferObjects.AuthenticationDtos.Request;
using KurumiConcursos.ApplicationService.DataTransferObjects.AuthenticationDtos.Response;
using KurumiConcursos.Domain.Entities;

namespace KurumiConcursos.ApplicationService.Interfaces.MapperContracts;

public interface IUserMapper
{
    User DtoRegisterToDomain(RegisterRequest request, Guid roleId);
    AuthenticationResponse DomainToAuthenticationResponse(User user, string token);
}
