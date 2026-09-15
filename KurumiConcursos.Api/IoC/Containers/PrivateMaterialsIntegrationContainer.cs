using System.Net.Http.Headers;
using System.Text;
using KurumiConcursos.Api.Services;
using KurumiConcursos.ApplicationService.Services.PrivateMaterialServices;
using KurumiConcursos.Domain.Providers;

namespace KurumiConcursos.Api.IoC.Containers;

public static class PrivateMaterialsIntegrationContainer
{
    public static IServiceCollection AddPrivateMaterialsIntegration(this IServiceCollection services)
    {
        services.AddHttpClient<INextcloudWebDavService, NextcloudWebDavService>((provider, client) =>
        {
            var options = provider.GetRequiredService<NextcloudOptions>();
            if (!Uri.TryCreate(options.Url, UriKind.Absolute, out var uri) || uri.Scheme != Uri.UriSchemeHttps ||
                string.IsNullOrWhiteSpace(options.User) || string.IsNullOrWhiteSpace(options.AppPassword))
                throw new InvalidOperationException("Nextcloud:Url, Nextcloud:User e Nextcloud:AppPassword devem ser configurados.");
            client.Timeout = TimeSpan.FromSeconds(Math.Clamp(options.TimeoutSeconds, 10, 300));
            client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Basic",
                Convert.ToBase64String(Encoding.UTF8.GetBytes($"{options.User}:{options.AppPassword}")));
        }).ConfigurePrimaryHttpMessageHandler(() => new HttpClientHandler { AllowAutoRedirect = false });
        return services;
    }
}
