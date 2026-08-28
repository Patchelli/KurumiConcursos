using System.Text;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;

namespace KurumiConcursos.Api.Settings.Handlers;

public static class AuthenticationSettings
{
    public static IServiceCollection AddAuthenticationSettings(this IServiceCollection services,
        IConfiguration configuration)
    {
        var jwt = configuration.GetSection("Jwt");
        var key = jwt["Key"];
        var issuer = jwt["Issuer"];
        var audience = jwt["Audience"];
        if (string.IsNullOrWhiteSpace(key) || string.IsNullOrWhiteSpace(issuer) ||
            string.IsNullOrWhiteSpace(audience))
            throw new InvalidOperationException("Jwt:Key, Jwt:Issuer e Jwt:Audience devem ser configurados.");
        services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme).AddJwtBearer(options =>
        {
            options.RequireHttpsMetadata = jwt.GetValue("RequireHttpsMetadata", true);
            options.TokenValidationParameters = new TokenValidationParameters
            {
                ValidateIssuer = true, ValidateAudience = true, ValidateLifetime = true,
                ValidateIssuerSigningKey = true, ValidIssuer = issuer, ValidAudience = audience,
                IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(key)),
                ClockSkew = TimeSpan.FromMinutes(1)
            };
        });
        services.AddAuthorization();
        return services;
    }
}
