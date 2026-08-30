using KurumiConcursos.ApplicationService.Interfaces.MapperContracts;
using KurumiConcursos.ApplicationService.Mappers;

namespace KurumiConcursos.Api.IoC.Containers;

public static class MapperContainer
{
    public static IServiceCollection AddMapperContainer(this IServiceCollection services) => services
        .AddTransient<IPersonalDataMapper, PersonalDataMapper>()
        .AddTransient<IUserMapper, UserMapper>()
        .AddTransient<IJourneyMapper, JourneyMapper>()
        .AddTransient<ICalendarEventMapper, CalendarEventMapper>();
}