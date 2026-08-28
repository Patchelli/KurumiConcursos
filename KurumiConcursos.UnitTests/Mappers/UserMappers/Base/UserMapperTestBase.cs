using KurumiConcursos.ApplicationService.Mappers;

namespace KurumiConcursos.UnitTests.Mappers.UserMappers.Base;

public abstract class UserMapperTestBase
{
    protected readonly UserMapper Mapper = new();
}