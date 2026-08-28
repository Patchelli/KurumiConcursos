using System.Security.Claims;
using KurumiConcursos.Domain.ValueObjects;

namespace KurumiConcursos.Api.Extensions;

public static class ClaimsPrincipalExtension
{
    public static UserCredential GetUserCredential(this ClaimsPrincipal user) =>
        new(Guid.Parse(user.FindFirstValue(ClaimTypes.NameIdentifier)!));
}
