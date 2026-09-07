using KurumiConcursos.Api.Settings.Constants;
using KurumiConcursos.Domain.Providers;

namespace KurumiConcursos.Api.Settings.Handlers;

public static class CorsSettings
{
    public static IServiceCollection AddCorsSettings(this IServiceCollection services, IConfiguration configuration)
    {
        var corsConfiguration = configuration.GetSection(CorsConfigurationOptions.SectionName)
            .Get<CorsConfigurationOptions>()!;
        var allowedOrigins = new[] { corsConfiguration.Web, corsConfiguration.Mobile }
            .Concat(corsConfiguration.AdditionalOrigins)
            .Where(origin => !string.IsNullOrWhiteSpace(origin))
            .Select(origin => origin.Trim().TrimEnd('/'))
            .Distinct(StringComparer.OrdinalIgnoreCase)
            .ToArray();

        services.AddCors(options => options.AddPolicy(CorsName.DefaultPolicy, builder =>
        {
            builder.WithOrigins(allowedOrigins)
                .WithMethods(corsConfiguration.Methods)
                .AllowAnyHeader()
                .AllowCredentials();
        }));
        return services;
    }
}
