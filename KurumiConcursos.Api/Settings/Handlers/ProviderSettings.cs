using KurumiConcursos.Api.Extensions;
using KurumiConcursos.Domain.Providers;

namespace KurumiConcursos.Api.Settings.Handlers;

public static class ProviderSettings
{
    public static void AddProviderSettings(this IServiceCollection services, IConfiguration configuration)
    {
        services.SetConfigureOptions<ConnectionStringOptions>(configuration, ConnectionStringOptions.SectionName);
        services.SetConfigureOptions<JwtOptions>(configuration, JwtOptions.SectionName);
    }
}