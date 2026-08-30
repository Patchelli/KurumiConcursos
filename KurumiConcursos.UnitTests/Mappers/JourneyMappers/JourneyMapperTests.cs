using KurumiConcursos.ApplicationService.DataTransferObjects.JourneyDtos.Request;
using KurumiConcursos.Domain.Enums;
using KurumiConcursos.UnitTests.Mappers.JourneyMappers.Base;

namespace KurumiConcursos.UnitTests.Mappers.JourneyMappers;

public sealed class JourneyMapperTests : JourneyMapperTestBase
{
    [Fact]
    public void DtoToJourney_MustKeepOwnerAndBusinessData()
    {
        var owner = Guid.NewGuid();
        var journey = Mapper.DtoRegisterToDomain(owner, new JourneyRegisterRequest("Receita Federal", "RFB", "FGV",
            "Auditor", 25000, 200, null, null,
            EJourneyStage.PreNotice, true, null, []));
        Assert.Equal(owner, journey.UserId);
        Assert.Equal("Receita Federal", journey.Title);
        Assert.Equal("Auditor", journey.Position);
    }
}