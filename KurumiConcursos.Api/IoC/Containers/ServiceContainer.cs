using KurumiConcursos.Api.Settings.Handlers;
using KurumiConcursos.ApplicationService.Interfaces.ServiceContracts;
using KurumiConcursos.ApplicationService.Services.AuthenticationServices;
using KurumiConcursos.ApplicationService.Services.CalendarServices;
using KurumiConcursos.ApplicationService.Services.FlashcardServices;
using KurumiConcursos.ApplicationService.Services.JourneyServices;
using KurumiConcursos.ApplicationService.Services.LoggerHandlerServices;
using KurumiConcursos.ApplicationService.Services.StudyResourceServices;
using KurumiConcursos.ApplicationService.Services.StudyRoutineServices;
using KurumiConcursos.ApplicationService.Services.StudyTimerServices;
using KurumiConcursos.ApplicationService.Services.SyllabusNodeStudyServices;
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
        .AddScoped<ICalendarEventCommandService, CalendarEventCommandService>()
        .AddScoped<ICalendarEventQueryService, CalendarEventQueryService>()
        .AddScoped<IStudyRoutineCommandService, StudyRoutineCommandService>()
        .AddScoped<IStudyRoutineQueryService, StudyRoutineQueryService>()
        .AddScoped<IStudyResourceCommandService, StudyResourceCommandService>()
        .AddScoped<IStudyResourceQueryService, StudyResourceQueryService>()
        .AddScoped<IStudyTimerCommandService, StudyTimerCommandService>()
        .AddScoped<IStudyTimerQueryService, StudyTimerQueryService>()
        .AddScoped<IFlashcardCommandService, FlashcardCommandService>()
        .AddScoped<IFlashcardQueryService, FlashcardQueryService>()
        .AddScoped<ISyllabusNodeStudyCommandService, SyllabusNodeStudyCommandService>()
        .AddScoped<ISyllabusNodeStudyQueryService, SyllabusNodeStudyQueryService>()
        .AddScoped<IUserQueryService, UserQueryService>()
        .AddScoped<IUserCommandService, UserCommandService>()
        .AddSingleton<ITokenService, JwtTokenService>();
}
