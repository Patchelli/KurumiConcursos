using KurumiConcursos.Api.Settings.Constants;

namespace KurumiConcursos.Api.Settings.Handlers;

public static class CorsSettings
{
    public static IServiceCollection AddCorsSettings(this IServiceCollection services, IConfiguration configuration)
    {
        var origins = configuration.GetSection("Cors:AllowedOrigins").Get<string[]>() ?? [];
        var methods = configuration.GetSection("Cors:AllowedMethods").Get<string[]>() ?? [];
        if (origins.Length == 0)
            throw new InvalidOperationException("Cors:AllowedOrigins deve conter ao menos uma origem.");
        services.AddCors(options => options.AddPolicy(CorsName.DefaultPolicy, policy =>
        {
            policy.WithOrigins(origins).AllowAnyHeader();
            if (methods.Length == 0) policy.AllowAnyMethod();
            else policy.WithMethods(methods);
        }));
        return services;
    }
}
