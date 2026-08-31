using KurumiConcursos.Infra.Interfaces.RepositoryContracts;
using KurumiConcursos.Infra.Repositories;

namespace KurumiConcursos.Api.IoC.Containers;

public static class RepositoryContainer
{
    public static IServiceCollection AddRepositoryContainer(this IServiceCollection services) => services
        .AddScoped<IUserRepository, UserRepository>()
        .AddScoped<IUserAuthenticationRepository, UserAuthenticationRepository>()
        .AddScoped<IRoleRepository, RoleRepository>()
        .AddScoped<IPersonalDataRepository, PersonalDataRepository>()
        .AddScoped<IStudentProfileRepository, StudentProfileRepository>()
        .AddScoped<IJourneyRepository, JourneyRepository>()
        .AddScoped<ICalendarEventRepository, CalendarEventRepository>()
        .AddScoped<IStudyRoutineRepository, StudyRoutineRepository>()
        .AddScoped<IStudyRoutineBlockRepository, StudyRoutineBlockRepository>()
        .AddScoped<IStudyResourceRepository, StudyResourceRepository>()
        .AddScoped<IFocusSessionRepository, FocusSessionRepository>()
        .AddScoped<IReviewAppointmentRepository, ReviewAppointmentRepository>();
}