using KurumiConcursos.ApplicationService.Mappers;

namespace KurumiConcursos.UnitTests.Mappers.JourneyMappers.Base;

public abstract class JourneyMapperTestBase
{
    protected readonly JourneyMapper Mapper = new();
}