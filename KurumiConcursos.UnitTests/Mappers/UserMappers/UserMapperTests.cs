using KurumiConcursos.ApplicationService.DataTransferObjects.AuthenticationDtos.Request;
using KurumiConcursos.UnitTests.Mappers.UserMappers.Base;

namespace KurumiConcursos.UnitTests.Mappers.UserMappers;

public sealed class UserMapperTests : UserMapperTestBase
{
    [Fact]
    public void DtoRegisterToDomain_MustMapAccount()
    {
        var request = new RegisterRequest("Ana", "ana@kurumi_concursos.test", "Senha123!");
        var account = Mapper.DtoRegisterToDomain(request);
        Assert.Equal(request.Name, account.Name);
        Assert.Equal(request.Email, account.Email);
        Assert.Null(account.PasswordHash);
    }
}