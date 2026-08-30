using KurumiConcursos.ApplicationService.DataTransferObjects.AuthenticationDtos.Request;
using KurumiConcursos.ApplicationService.DataTransferObjects.PersonalDataDtos.Request;
using KurumiConcursos.UnitTests.Mappers.UserMappers.Base;

namespace KurumiConcursos.UnitTests.Mappers.UserMappers;

public sealed class UserMapperTests : UserMapperTestBase
{
    [Fact]
    public void DtoRegisterToDomain_MustMapAccount()
    {
        var request = new RegisterRequest
        {
            Email = "ana@kurumi_concursos.test",
            Password = "Senha123!",
            PersonalData = new PersonalDataRegisterRequest { FullName = "Ana" }
        };
        var account = Mapper.DtoRegisterToDomain(request, Guid.NewGuid());
        Assert.Equal(request.PersonalData.FullName, account.PersonalData?.FullName);
        Assert.Equal(request.Email, account.Email);
        Assert.Null(account.PasswordHash);
    }
}