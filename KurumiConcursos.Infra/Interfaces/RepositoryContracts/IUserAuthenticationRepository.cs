using KurumiConcursos.Domain.Entities;
using Microsoft.AspNetCore.Identity;

namespace KurumiConcursos.Infra.Interfaces.RepositoryContracts;

public interface IUserAuthenticationRepository
{
    Task<SignInResult> UserAuthenticationAsync(string login, string password);
    Task<SignInResult> UserAuthenticationAsync(User user, string password);
    Task UserSignOutAsync();
}