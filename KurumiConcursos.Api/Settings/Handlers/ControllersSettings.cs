using System.Text.Json.Serialization;

namespace KurumiConcursos.Api.Settings.Handlers;

public static class ControllersSettings
{
    public static IServiceCollection AddControllersSettings(this IServiceCollection services)
    {
        services.AddControllers().AddJsonOptions(options =>
            options.JsonSerializerOptions.Converters.Add(new JsonStringEnumConverter()));
        services.AddEndpointsApiExplorer();
        return services;
    }
}
