using System.Security.Claims;
using KurumiConcursos.Domain.ValueObjects;

namespace KurumiConcursos.Api.Extensions;

public static class ClaimsPrincipalExtension
{
    public static string GetEmail(this ClaimsPrincipal user) =>
        user.FindFirst(ClaimTypes.Email)?.Value!;

    public static string GetName(this ClaimsPrincipal user) =>
        user.FindFirst(ClaimTypes.Name)?.Value!;

    public static Guid GetUserId(this ClaimsPrincipal user) =>
        Guid.Parse(user.FindFirst(ClaimTypes.NameIdentifier)?.Value!);

    public static UserCredential GetUserCredential(this ClaimsPrincipal user)
    {
        var roles = user.FindAll(ClaimTypes.Role);
        var id = Guid.Parse(user.FindFirst(ClaimTypes.NameIdentifier)?.Value!);

        return new UserCredential
        {
            UserId = id,
            Roles = roles.Select(role => role.Value).ToList()
        };
    }

    public static UserFullData GetUserFullData(this ClaimsPrincipal user)
    {
        var roles = user.FindAll(ClaimTypes.Role);
        var id = Guid.Parse(user.FindFirst(ClaimTypes.NameIdentifier)?.Value!);
        var name = user.FindFirst(ClaimTypes.Name)?.Value!;

        return new UserFullData
        {
            Id = id,
            Name = name,
            Roles = roles.Select(role => role.Value).ToList()
        };
    }

    public static UserCredential? TryGetUserCredential(this ClaimsPrincipal user)
    {
        if (user?.Identity?.IsAuthenticated != true)
            return null;

        var idValue = user.FindFirst(ClaimTypes.NameIdentifier)?.Value;

        if (string.IsNullOrWhiteSpace(idValue))
            return null;

        if (!Guid.TryParse(idValue, out var id))
            return null;

        var roles = user.FindAll(ClaimTypes.Role);

        return new UserCredential
        {
            UserId = id,
            Roles = roles.Select(role => role.Value).ToList()
        };
    }
}