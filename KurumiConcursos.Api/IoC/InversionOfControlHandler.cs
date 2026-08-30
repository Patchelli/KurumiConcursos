using KurumiConcursos.Api.IoC.Containers;
using KurumiConcursos.Domain.Handlers.NotificationHandler;
using KurumiConcursos.Domain.Interface;
using KurumiConcursos.Infra.Interfaces.ServiceContracts;
using KurumiConcursos.Infra.ORM.UoW;
using KurumiConcursos.Infra.Services;

namespace KurumiConcursos.Api.IoC;

public static class InversionOfControlHandler
{
    public static IServiceCollection AddInversionOfControlHandler(this IServiceCollection s) => s
        .AddScoped<IUnitOfWork, UnitOfWork>()
        .AddScoped<INotificationHandler, NotificationHandler>()
        .AddScoped(typeof(IPaginationQueryService<>), typeof(PaginationQueryService<>))
        .AddServiceContainer().AddMapperContainer().AddRepositoryContainer()
        .AddValidationContainer();
}