using System.Text;
using KurumiConcursos.Domain.Entities;
using KurumiConcursos.Domain.Entities.IdentityEntities;
using KurumiConcursos.Domain.Extensions;
using KurumiConcursos.Infra.ORM.Context;
using Microsoft.AspNetCore.Identity;

namespace KurumiConcursos.Api.Settings.Handlers;

public static class IdentitySettings
{
    public static IServiceCollection AddIdentitySettings(this IServiceCollection services)
    {
        services.AddIdentityCore<User>(options =>
        {
            options.SignIn.RequireConfirmedAccount = false;
            options.SignIn.RequireConfirmedPhoneNumber = false;
            options.Password.RequiredLength = 8;
            options.Password.RequireDigit = false;
            options.Password.RequireLowercase = false;
            options.Password.RequireUppercase = false;
            options.Password.RequireNonAlphanumeric = false;
            options.Lockout.MaxFailedAccessAttempts = 5;
            options.Lockout.DefaultLockoutTimeSpan = TimeSpan.FromMinutes(5);
            options.User.AllowedUserNameCharacters = EncodingExtension.GetAllWritableCharacters(Encoding.UTF8);
        }).AddRoles<Role>()
            .AddRoleManager<RoleManager<Role>>()
            .AddEntityFrameworkStores<ApplicationContext>()
            .AddSignInManager<SignInManager<User>>()
            .AddRoleValidator<RoleValidator<Role>>()
            .AddDefaultTokenProviders();
        return services;
    }
}
