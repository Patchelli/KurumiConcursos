using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Microsoft.IdentityModel.Tokens;
using KurumiConcursos.ApplicationService.Interfaces.ServiceContracts;
using KurumiConcursos.Domain.Providers;
using KurumiConcursos.Domain.Entities;

namespace KurumiConcursos.Api.Settings.Handlers;

public sealed class JwtTokenService(JwtOptions jwt) : ITokenService
{
    public string Create(User user)
    {
        var durationInMinutes = jwt.DurationInMinutes;
        if (durationInMinutes <= 0)
            throw new InvalidOperationException("Jwt:DurationInMinutes deve ser maior que zero.");
        var claims = new List<Claim>
        {
            new(JwtRegisteredClaimNames.Sub, user.Id.ToString()),
            new(ClaimTypes.NameIdentifier, user.Id.ToString()),
            new(ClaimTypes.Name, user.PersonalData?.FullName ?? string.Empty),
            new(ClaimTypes.Email, user.Email ?? string.Empty)
        };

        var roles = (user.UserRoles ?? [])
            .Where(userRole => !string.IsNullOrWhiteSpace(userRole.Role?.Name))
            .Select(userRole => userRole.Role!.Name!)
            .Distinct(StringComparer.OrdinalIgnoreCase)
            .ToList();

        foreach (var role in roles)
        {
            claims.Add(new Claim(ClaimTypes.Role, role));
        }

        if (roles.Count > 0)
            claims.Add(new Claim("profile", roles[0]));

        var credentials = new SigningCredentials(new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwt.JwtKey)),
            SecurityAlgorithms.HmacSha256);
        return new JwtSecurityTokenHandler().WriteToken(new JwtSecurityToken(jwt.Issuer, jwt.Audience, claims,
            expires: DateTime.UtcNow.AddMinutes(durationInMinutes), signingCredentials: credentials));
    }
}
