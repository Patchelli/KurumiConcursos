using KurumiConcursos.Api.Settings.Handlers;

namespace KurumiConcursos.Api.Settings;

public static class SettingsControl
{
    public static void AddSettingsControl(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddMemoryCache();
        services.AddLocalization();
        services.AddProviderSettings(configuration);
        services.AddControllersSettings();
        services.AddCorsSettings(configuration);
        services.AddDatabaseConnectionSettings();
        services.AddIdentitySettings();
        services.AddAuthenticationSettings(configuration);
        services.AddFiltersSettings();
        services.AddSwaggerSettings();
        services.AddRateLimitingSettings();
    }
}