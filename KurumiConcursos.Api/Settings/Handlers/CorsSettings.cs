using KurumiConcursos.Api.Settings.Constants;
using KurumiConcursos.Domain.Providers;

namespace KurumiConcursos.Api.Settings.Handlers;

public static class CorsSettings
{
    public static IServiceCollection AddCorsSettings(this IServiceCollection services, IConfiguration configuration)
    {
        var frontConfiguration = configuration.GetSection(FrontConfigurationOptions.SectionName)
            .Get<FrontConfigurationOptions>();
        services.AddCors(options => options.AddPolicy(CorsName.DefaultPolicy, builder =>
        {
            builder.WithMethods(frontConfiguration!.Methods)
                .AllowAnyHeader()
                .SetIsOriginAllowed(_ => true)
                .AllowCredentials();
        }));
        return services;
    }
}