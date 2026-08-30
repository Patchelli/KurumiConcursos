using KurumiConcursos.Domain.Handlers.NotificationHandler;
using Microsoft.AspNetCore.Identity;

namespace KurumiConcursos.Domain.Extensions;

public static class IdentityExtension
{
    public static IEnumerable<DomainNotification> SetNotificationByIdentityResult(
        this IdentityResult identityResult, string? trace = null) =>
        identityResult.Errors.Select(error => new DomainNotification(
            trace ?? "Identity Error",
            error.Description)).ToList();

    public static string SetNotificationBySignInResult(this SignInResult signInResult, string? trace = null)
    {
        if (signInResult.IsLockedOut)
            return "Atenção! Este usuário está bloqueado. Consulte o suporte";
        if (signInResult.IsNotAllowed)
            return "Usuário sem permissão";
        return signInResult.RequiresTwoFactor
            ? "Requer autenticação 2FA"
            : "Usuário ou senha incorretos";
    }
}