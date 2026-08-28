using Microsoft.EntityFrameworkCore;
using KurumiConcursos.Infra.ORM.Context;

namespace KurumiConcursos.Api.Settings.Handlers;

public static class DatabaseConnectionSettings
{
    public static IServiceCollection AddDatabaseConnectionSettings(this IServiceCollection services,
        IConfiguration configuration)
    {
        var connection = configuration.GetConnectionString("Default") ??
                         throw new InvalidOperationException("ConnectionStrings:Default não configurada.");
        services.AddDbContext<ApplicationContext>(options => options.UseNpgsql(connection,
            npgsql => npgsql.MigrationsAssembly(typeof(ApplicationContext).Assembly.FullName)));
        return services;
    }
}