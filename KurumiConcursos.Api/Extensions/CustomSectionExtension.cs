using Microsoft.Extensions.Options;

namespace KurumiConcursos.Api.Extensions;

public static class CustomSectionExtension
{
    public static void SetConfigureOptions<T>(
        this IServiceCollection services,
        IConfiguration configuration,
        string sectionName) where T : class
    {
        services.Configure<T>(configuration.GetSection(sectionName));
        services.AddTransient(sp => sp.GetRequiredService<IOptionsMonitor<T>>().CurrentValue);
    }
}
