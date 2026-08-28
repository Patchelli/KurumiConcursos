using Microsoft.EntityFrameworkCore;
using KurumiConcursos.Infra.ORM.Context;

namespace KurumiConcursos.Api.Settings.Handlers;

public static class MigrationHandlerSettings
{
    public static async Task MigrateDatabaseAsync(this WebApplication app)
    {
        if (!app.Environment.IsDevelopment()) return;
        await using var scope = app.Services.CreateAsyncScope();
        await scope.ServiceProvider.GetRequiredService<ApplicationContext>().Database.MigrateAsync();
    }
}