using Microsoft.EntityFrameworkCore;
using KurumiConcursos.Infra.ORM.Context;
using KurumiConcursos.Domain.Providers;

namespace KurumiConcursos.Api.Settings.Handlers;

public static class DatabaseConnectionSettings
{
    public static IServiceCollection AddDatabaseConnectionSettings(this IServiceCollection services)
    {
        services.AddDbContextPool<ApplicationContext>((serviceProvider, options) =>
            options.UseNpgsql(
                serviceProvider.GetRequiredService<ConnectionStringOptions>().DefaultConnection,
                npgsql =>
                {
                    npgsql.MigrationsAssembly(typeof(ApplicationContext).Assembly.FullName);
                    npgsql.CommandTimeout(180);
                }));
        return services;
    }
}
