using KurumiConcursos.Api.Settings.Constants;

namespace KurumiConcursos.Api.Settings.Handlers;

public static class CorsSettings
{
    public static IServiceCollection AddCorsSettings(this IServiceCollection services, IConfiguration configuration)
    {
        var origins = configuration.GetSection("Cors:AllowedOrigins").Get<string[]>() ?? ["http://localhost:5173"];
        services.AddCors(options => options.AddPolicy(CorsName.DefaultPolicy,
            policy => policy.WithOrigins(origins).AllowAnyHeader().AllowAnyMethod()));
        return services;
    }
}