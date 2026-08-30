using KurumiConcursos.Api.Settings.Handlers;
using KurumiConcursos.ApplicationService.Interfaces.ServiceContracts;
using KurumiConcursos.ApplicationService.Services.AuthenticationServices;
using KurumiConcursos.ApplicationService.Services.CalendarServices;
using KurumiConcursos.ApplicationService.Services.JourneyServices;
using KurumiConcursos.ApplicationService.Services.LoggerHandlerServices;
using KurumiConcursos.ApplicationService.Services.UserServices;
using KurumiConcursos.Domain.Interface;

namespace KurumiConcursos.Api.IoC.Containers;

public static class ServiceContainer
{
    public static IServiceCollection AddServiceContainer(this IServiceCollection services) => services
        .AddScoped<ILoggerHandler, LoggerHandler>()
        .AddScoped<IAuthenticationCommandService, AuthenticationCommandService>()
        .AddScoped<IJourneyCommandService, JourneyCommandService>()
        .AddScoped<IJourneyQueryService, JourneyQueryService>()
        .AddScoped<ICalendarEventCommandService, CalendarEventService>()
        .AddScoped<ICalendarEventQueryService, CalendarEventService>()
        .AddScoped<IUserQueryService, UserQueryService>()
        .AddScoped<IUserCommandService, UserCommandService>()
        .AddSingleton<ITokenService, JwtTokenService>();
}