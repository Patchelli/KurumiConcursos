using KurumiConcursos.ApplicationService.DataTransferObjects.AuthenticationDtos.Request;
using KurumiConcursos.ApplicationService.DataTransferObjects.AuthenticationDtos.Response;
using KurumiConcursos.ApplicationService.Interfaces.MapperContracts;
using KurumiConcursos.Domain.Entities;
using KurumiConcursos.Domain.Enums;

namespace KurumiConcursos.ApplicationService.Mappers;

public sealed class UserMapper : IUserMapper
{
    public User DtoRegisterToDomain(RegisterRequest request) => new()
    {
        Id = Guid.NewGuid(),
        Name = request.Name,
        UserName = request.Email,
        NormalizedUserName = request.Email.ToUpperInvariant(),
        Email = request.Email,
        NormalizedEmail = request.Email.ToUpperInvariant(),
        EmailConfirmed = false,
        Status = EUserStatus.Active,
        CreationDate = DateTimeOffset.UtcNow
    };

    public AuthenticationResponse DomainToAuthenticationResponse(User user, string accessToken) =>
        new(accessToken, user.Name, user.Email!);
}
