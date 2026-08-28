using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Microsoft.IdentityModel.Tokens;
using KurumiConcursos.ApplicationService.Interfaces.ServiceContracts;

namespace KurumiConcursos.Api.Settings.Handlers;

public sealed class JwtTokenService(IConfiguration configuration) : ITokenService
{
    public string Create(Guid accountId, string name, string email)
    {
        var jwt = configuration.GetSection("Jwt");
        var durationInMinutes = jwt.GetValue("DurationInMinutes", 480);
        if (durationInMinutes <= 0)
            throw new InvalidOperationException("Jwt:DurationInMinutes deve ser maior que zero.");
        var claims = new[]
        {
            new Claim(JwtRegisteredClaimNames.Sub, accountId.ToString()),
            new Claim(ClaimTypes.NameIdentifier, accountId.ToString()), new Claim(ClaimTypes.Name, name),
            new Claim(ClaimTypes.Email, email)
        };
        var credentials = new SigningCredentials(new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwt["Key"]!)),
            SecurityAlgorithms.HmacSha256);
        return new JwtSecurityTokenHandler().WriteToken(new JwtSecurityToken(jwt["Issuer"], jwt["Audience"], claims,
            expires: DateTime.UtcNow.AddMinutes(durationInMinutes), signingCredentials: credentials));
    }
}
