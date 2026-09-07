using KurumiConcursos.ApplicationService.Interfaces.MapperContracts;
using KurumiConcursos.ApplicationService.Mappers;
using KurumiConcursos.Infra.Interfaces.MapperContracts;
using KurumiConcursos.Infra.Mappers;

namespace KurumiConcursos.Api.IoC.Containers;

public static class MapperContainer
{
    public static IServiceCollection AddMapperContainer(this IServiceCollection services) => services
        .AddTransient<IPersonalDataMapper, PersonalDataMapper>()
        .AddTransient<IUserMapper, UserMapper>()
        .AddTransient<IRadarMapper, RadarMapper>()
        .AddTransient<IPciContestMapper, PciContestMapper>()
        .AddTransient<IJourneyMapper, JourneyMapper>()
        .AddTransient<IJourneyOverviewMapper, JourneyOverviewMapper>()
        .AddTransient<ICalendarEventMapper, CalendarEventMapper>()
        .AddTransient<IStudyRoutineMapper, StudyRoutineMapper>()
        .AddTransient<IStudyResourceMapper, StudyResourceMapper>()
        .AddTransient<IFlashcardMapper, FlashcardMapper>()
        .AddTransient<IStudyTimerMapper, StudyTimerMapper>()
        .AddTransient<ISyllabusNodeStudyMapper, SyllabusNodeStudyMapper>()
        .AddTransient<IQuestionAppointmentMapper, QuestionAppointmentMapper>()
        .AddTransient<IPerformanceAdaptationMapper, PerformanceAdaptationMapper>()
        .AddTransient<ITimeCapsuleMapper, TimeCapsuleMapper>();
}