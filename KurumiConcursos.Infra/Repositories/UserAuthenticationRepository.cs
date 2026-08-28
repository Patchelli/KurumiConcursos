using KurumiConcursos.Domain.Entities;
using KurumiConcursos.Infra.Interfaces.RepositoryContracts;
using Microsoft.AspNetCore.Identity;

namespace KurumiConcursos.Infra.Repositories;

public sealed class UserAuthenticationRepository(SignInManager<User> signInManager)
    : IUserAuthenticationRepository
{
    public Task<SignInResult> UserAuthenticationAsync(string login, string password) =>
        signInManager.PasswordSignInAsync(login, password, false, true);
    public Task<SignInResult> UserAuthenticationAsync(User user, string password) =>
        signInManager.CheckPasswordSignInAsync(user, password, lockoutOnFailure: true);
    public Task UserSignOutAsync() => signInManager.SignOutAsync();
}
