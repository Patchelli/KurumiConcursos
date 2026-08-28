using KurumiConcursos.Api.IoC.Containers;
using KurumiConcursos.Domain.Interface;
using KurumiConcursos.Infra.ORM.UoW;
using KurumiConcursos.Infra.Interfaces.ServiceContracts;
using KurumiConcursos.Infra.Services;
using KurumiConcursos.Domain.Handlers.NotificationHandler;
using KurumiConcursos.Infra.Diagnostics;

namespace KurumiConcursos.Api.IoC;

public static class InversionOfControlHandler
{
    public static IServiceCollection AddInversionOfControlHandler(this IServiceCollection s) => s
        .AddScoped<TemporaryJourneyPerformanceProbe>() // TEMP-PERF-JOURNEY
        .AddScoped<IUnitOfWork, UnitOfWork>()
        .AddScoped<INotificationHandler, NotificationHandler>()
        .AddScoped(typeof(IPaginationQueryService<>), typeof(PaginationQueryService<>))
        .AddServiceContainer().AddMapperContainer().AddRepositoryContainer()
        .AddValidationContainer();
}
