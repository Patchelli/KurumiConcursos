using System.Text.Json.Serialization;
using KurumiConcursos.Api.Filters;

namespace KurumiConcursos.Api.Settings.Handlers;

public static class ControllersSettings
{
    public static IServiceCollection AddControllersSettings(this IServiceCollection services)
    {
        services.AddScoped<TemporaryJourneyPerformanceResultFilter>();
        services.AddControllers(options =>
            options.Filters.AddService<TemporaryJourneyPerformanceResultFilter>()).AddJsonOptions(options =>
            options.JsonSerializerOptions.Converters.Add(new JsonStringEnumConverter()));
        services.AddEndpointsApiExplorer();
        return services;
    }
}
