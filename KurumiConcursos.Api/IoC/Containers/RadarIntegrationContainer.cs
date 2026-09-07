using KurumiConcursos.Domain.Providers;
using KurumiConcursos.Infra.Interfaces.ServiceContracts;
using KurumiConcursos.Infra.Services.PciContestServices;

namespace KurumiConcursos.Api.IoC.Containers;

public static class RadarIntegrationContainer
{
    public static IServiceCollection AddRadarIntegrationContainer(this IServiceCollection services)
    {
        services.AddHttpClient<IPciContestQueryService, PciContestQueryService>((provider, client) =>
        {
            var options = provider.GetRequiredService<PciConcursosOptions>();
            client.BaseAddress = new Uri(options.ServerUrl);
            client.Timeout = TimeSpan.FromSeconds(options.TimeoutSeconds);
            client.DefaultRequestHeaders.Accept.ParseAdd("application/json");
            client.DefaultRequestHeaders.Accept.ParseAdd("text/event-stream");
        });
        return services;
    }
}