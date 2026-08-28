using KurumiConcursos.Domain.Entities;
using KurumiConcursos.Domain.Entities.IdentityEntities;
using KurumiConcursos.Infra.ORM.Context;
using Microsoft.AspNetCore.Identity;

namespace KurumiConcursos.Api.Settings.Handlers;

public static class IdentitySettings
{
    public static IServiceCollection AddIdentitySettings(this IServiceCollection services)
    {
        services.AddIdentityCore<User>(options =>
        {
            options.Password.RequiredLength = 8;
            options.Password.RequireDigit = true;
            options.Password.RequireUppercase = true;
            options.Password.RequireNonAlphanumeric = false;
            options.Lockout.MaxFailedAccessAttempts = 5;
            options.User.RequireUniqueEmail = true;
        }).AddRoles<Role>().AddEntityFrameworkStores<ApplicationContext>().AddSignInManager<SignInManager<User>>();
        return services;
    }
}
