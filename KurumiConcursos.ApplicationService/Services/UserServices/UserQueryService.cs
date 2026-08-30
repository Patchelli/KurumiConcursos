using KurumiConcursos.ApplicationService.DataTransferObjects.UserDtos.Response;
using KurumiConcursos.ApplicationService.Interfaces.MapperContracts;
using KurumiConcursos.ApplicationService.Interfaces.ServiceContracts;
using KurumiConcursos.Domain.ValueObjects;
using KurumiConcursos.Infra.Interfaces.RepositoryContracts;
using Microsoft.EntityFrameworkCore;

namespace KurumiConcursos.ApplicationService.Services.UserServices;

public sealed class UserQueryService(IUserRepository userRepository, IUserMapper userMapper) : IUserQueryService
{
    public async Task<UserProfileResponse?> GetMyProfileAsync(UserCredential credential)
    {
        var user = await userRepository.FindByPredicateAsync(
            item => item.Id == credential.UserId,
            query => query.Include(item => item.PersonalData!).Include(item => item.UserRoles!));

        return user is null ? null : userMapper.DomainToUserProfileResponse(user);
    }
}
