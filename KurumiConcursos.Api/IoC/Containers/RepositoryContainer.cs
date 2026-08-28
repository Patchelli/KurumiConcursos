using KurumiConcursos.Infra.Interfaces.RepositoryContracts;
using KurumiConcursos.Infra.Repositories;

namespace KurumiConcursos.Api.IoC.Containers;

public static class RepositoryContainer
{
    public static IServiceCollection AddRepositoryContainer(this IServiceCollection services) => services
        .AddScoped<IUserRepository, UserRepository>()
        .AddScoped<IUserAuthenticationRepository, UserAuthenticationRepository>()
        .AddScoped<IJourneyRepository, JourneyRepository>();
}
