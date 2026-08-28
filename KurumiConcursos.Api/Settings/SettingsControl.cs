using KurumiConcursos.Api.Settings.Handlers;

namespace KurumiConcursos.Api.Settings;

public static class SettingsControl
{
    public static void AddSettingsControl(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddControllersSettings();
        services.AddCorsSettings(configuration);
        services.AddDatabaseConnectionSettings(configuration);
        services.AddIdentitySettings();
        services.AddAuthenticationSettings(configuration);
        services.AddFiltersSettings();
        services.AddSwaggerSettings();
    }
}
