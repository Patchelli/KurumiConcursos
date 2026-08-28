using System.Text;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Identity;
using Microsoft.IdentityModel.Tokens;
using KurumiConcursos.Domain.Providers;

namespace KurumiConcursos.Api.Settings.Handlers;

public static class AuthenticationSettings
{
    public static IServiceCollection AddAuthenticationSettings(this IServiceCollection services,
        IConfiguration configuration)
    {
        var jwt = configuration.GetSection(JwtOptions.SectionName).Get<JwtOptions>();
        var key = jwt?.JwtKey;
        var issuer = jwt?.Issuer;
        var audience = jwt?.Audience;
        if (string.IsNullOrWhiteSpace(key) || string.IsNullOrWhiteSpace(issuer) ||
            string.IsNullOrWhiteSpace(audience))
            throw new InvalidOperationException("Jwt:JwtKey, Jwt:Issuer e Jwt:Audience devem ser configurados.");
        services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme).AddJwtBearer(options =>
        {
            options.RequireHttpsMetadata = jwt!.RequireHttpsMetadata;
            options.SaveToken = true;
            options.TokenValidationParameters = new TokenValidationParameters
            {
                ValidateIssuer = true, ValidateAudience = true, ValidateLifetime = true,
                ValidateIssuerSigningKey = true, ValidIssuer = issuer, ValidAudience = audience,
                IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(key)),
                ClockSkew = TimeSpan.Zero
            };
        })
        .AddCookie(CookieAuthenticationDefaults.AuthenticationScheme)
        .AddCookie(IdentityConstants.ApplicationScheme);
        services.AddAuthorization();
        return services;
    }
}
