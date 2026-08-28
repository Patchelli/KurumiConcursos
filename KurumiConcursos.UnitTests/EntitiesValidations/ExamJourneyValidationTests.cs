using KurumiConcursos.Domain.Entities;
using KurumiConcursos.Domain.EntitiesValidation;

namespace KurumiConcursos.UnitTests.EntitiesValidations;

public sealed class ExamJourneyValidationTests
{
    [Fact]
    public void EmptyTitle_MustBeInvalid()
    {
        var entity = new ExamJourney { AccountId = Guid.NewGuid(), Title = string.Empty };
        var result = new ExamJourneyValidation().Validate(entity);
        Assert.False(result.IsValid);
    }
}