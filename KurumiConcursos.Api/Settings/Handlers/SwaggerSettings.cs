namespace KurumiConcursos.Api.Settings.Handlers;

public static class SwaggerSettings
{
    public static IServiceCollection AddSwaggerSettings(this IServiceCollection services)
    {
        services.AddSwaggerGen();
        return services;
    }
}