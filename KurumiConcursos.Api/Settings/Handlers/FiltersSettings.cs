using KurumiConcursos.Api.Filters;

namespace KurumiConcursos.Api.Settings.Handlers;

public static class FiltersSettings
{
    public static void AddFiltersSettings(this IServiceCollection services)
    {
        services.AddMvc(config => config.Filters.AddService<NotificationFilter>());
        services.AddMvc(config => config.Filters.AddService<UnitOfWorkFilter>());
        services.AddScoped<NotificationFilter>();
        services.AddScoped<UnitOfWorkFilter>();
    }
}