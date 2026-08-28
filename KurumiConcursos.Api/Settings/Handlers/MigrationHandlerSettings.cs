using Microsoft.EntityFrameworkCore;
using KurumiConcursos.Infra.ORM.Context;
using KurumiConcursos.Domain.Entities;
using KurumiConcursos.Infra.Interfaces.RepositoryContracts;
using Microsoft.AspNetCore.Identity;

namespace KurumiConcursos.Api.Settings.Handlers;

public static class MigrationHandlerSettings
{
    public static async Task MigrateDatabaseAsync(this WebApplication app)
    {
        await using var scope = app.Services.CreateAsyncScope();
        var context = scope.ServiceProvider.GetRequiredService<ApplicationContext>();
        var userRepository = scope.ServiceProvider.GetRequiredService<IUserRepository>();
        var passwordHasher = scope.ServiceProvider.GetRequiredService<IPasswordHasher<User>>();
        await context.Database.MigrateAsync();
        var seedHandler = new DbInitializer(context, userRepository, passwordHasher);
        await seedHandler.Seed();
    }
}
