using KurumiConcursos.ApplicationService.DataTransferObjects.AuthenticationDtos.Request;
using KurumiConcursos.ApplicationService.DataTransferObjects.AuthenticationDtos.Response;
using KurumiConcursos.ApplicationService.DataTransferObjects.UserDtos.Response;
using KurumiConcursos.ApplicationService.Interfaces.MapperContracts;
using KurumiConcursos.Domain.Entities;
using KurumiConcursos.Domain.Entities.IdentityEntities;
using KurumiConcursos.Domain.Enums;

namespace KurumiConcursos.ApplicationService.Mappers;

public sealed class UserMapper(IPersonalDataMapper personalDataMapper) : IUserMapper
{
    public User DtoRegisterToDomain(RegisterRequest request, Guid roleId)
    {
        var userId = Guid.NewGuid();
        return new User
        {
            Id = userId,
            UserName = request.Email,
            NormalizedUserName = request.Email.ToUpperInvariant(),
            Email = request.Email,
            NormalizedEmail = request.Email.ToUpperInvariant(),
            EmailConfirmed = false,
            PhoneNumber = request.PersonalData.Phone,
            PhoneNumberConfirmed = !string.IsNullOrWhiteSpace(request.PersonalData.Phone),
            CreationDate = DateTimeOffset.UtcNow,
            Status = EUserStatus.Active,
            PersonalData = personalDataMapper.DtoRegisterBasicToDomain(request.PersonalData),
            UserRoles = [new UserRole { RoleId = roleId }]
        };
    }

    public AuthenticationResponse DomainToAuthenticationResponse(User user, string accessToken) =>
        new(accessToken, user.PersonalData?.FullName ?? string.Empty, user.Email!);

    public UserProfileResponse DomainToUserProfileResponse(User user) =>
        new()
        {
            UserId = user.Id,
            Email = user.Email ?? string.Empty,
            Status = user.Status,
            Roles = user.UserRoles?.Where(item => item.Role is not null)
                .Select(item => item.Role!.Name ?? string.Empty)
                .Where(item => item.Length > 0).ToList() ?? [],
            PersonalData = user.PersonalData is null
                ? null
                : personalDataMapper.DomainToDtoResponse(user.PersonalData)
        };
}