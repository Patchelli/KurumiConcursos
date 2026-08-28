using KurumiConcursos.ApplicationService.DataTransferObjects.AuthenticationDtos.Request;
using KurumiConcursos.ApplicationService.DataTransferObjects.AuthenticationDtos.Response;
using KurumiConcursos.ApplicationService.Interfaces.MapperContracts;
using KurumiConcursos.ApplicationService.Interfaces.ServiceContracts;
using KurumiConcursos.Domain.Entities;
using KurumiConcursos.Domain.ValueObjects;
using KurumiConcursos.Domain.Extensions;
using KurumiConcursos.Domain.Interface;
using KurumiConcursos.Infra.Interfaces.RepositoryContracts;
using KurumiConcursos.ApplicationService.Traces;
using KurumiConcursos.Domain.Enums;
using Microsoft.EntityFrameworkCore;

namespace KurumiConcursos.ApplicationService.Services.AuthenticationServices;

public sealed class AuthenticationCommandService(
    IUserRepository userRepository,
    IUserAuthenticationRepository userAuthenticationRepository,
    IRoleRepository roleRepository,
    IStudentProfileRepository studentProfileRepository,
    IUserMapper userMapper,
    ITokenService tokenService,
    INotificationHandler notificationHandler) : IAuthenticationCommandService
{
    public async Task<AuthenticationResponse?> RegisterAsync(RegisterRequest request)
    {
        var normalizedEmail = request.Email.Trim().ToLowerInvariant();
        if ((request.PersonalData.FullName?.Trim().Length ?? 0) < 2 || !normalizedEmail.Contains('@') || request.Password.Length < 8)
        {
            notificationHandler.CreateNotification(UserTrace.Save,
                "Informe nome, e-mail válido e senha com ao menos 8 caracteres.");
            return null;
        }
        if (await userRepository.ExistsAsync(user => user.NormalizedEmail == normalizedEmail.ToUpperInvariant()))
        {
            notificationHandler.CreateNotification(UserTrace.Save, "E-mail já cadastrado.");
            return null;
        }

        var studentRole = await roleRepository.FindByPredicateAsync(
            role => role.Type == ERoleType.Student,
            toQuery: true);
        if (studentRole is null)
        {
            notificationHandler.CreateNotification(UserTrace.Save, "Perfil de estudante não configurado.");
            return null;
        }

        var user = userMapper.DtoRegisterToDomain(request with { Email = normalizedEmail }, studentRole.Id);
        user.PasswordHash = userRepository.HashPassword(user, request.Password);
        var creation = await userRepository.SaveAsync(user);
        if (!creation.Succeeded)
        {
            notificationHandler.CreateNotifications(
                creation.SetNotificationByIdentityResult(UserTrace.Save));
            return null;
        }

        if (!await studentProfileRepository.SaveAsync(new StudentProfile { UserId = user.Id }))
        {
            notificationHandler.CreateNotification(UserTrace.Save, "Não foi possível criar o perfil de estudante.");
            return null;
        }

        user.UserRoles![0].Role = studentRole;

        return CreateAuthentication(user);
    }

    public async Task<AuthenticationResponse?> CreateAccessTokenAsync(LoginRequest request)
    {
        var normalizedEmail = request.Email.Trim().ToUpperInvariant();
        var user = await userRepository.FindByPredicateAsync(
            candidate => candidate.NormalizedEmail == normalizedEmail,
            query => query
                .Include(candidate => candidate.PersonalData!)
                .Include(candidate => candidate.UserRoles!)
                .ThenInclude(userRole => userRole.Role!));
        if (user is null)
        {
            notificationHandler.CreateNotification(
                AuthenticationTrace.AccessOrRefreshToken,
                AuthenticationTrace.LoginOrPassword);
            return null;
        }

        var signInResult = await userAuthenticationRepository.UserAuthenticationAsync(user, request.Password);
        if (!signInResult.Succeeded)
        {
            notificationHandler.CreateNotification(
                AuthenticationTrace.AccessToken,
                signInResult.SetNotificationBySignInResult());
            return null;
        }

        user.LastAccessDate = DateTimeOffset.UtcNow;
        await userRepository.UpdateAsync(user);
        return CreateAuthentication(user);
    }

    public async Task<bool> LogoutAsync(UserCredential userCredential)
    {
        await userAuthenticationRepository.UserSignOutAsync();
        return true;
    }

    private AuthenticationResponse CreateAuthentication(User user)
    {
        var accessToken = tokenService.Create(user);
        return userMapper.DomainToAuthenticationResponse(user, accessToken);
    }
}
