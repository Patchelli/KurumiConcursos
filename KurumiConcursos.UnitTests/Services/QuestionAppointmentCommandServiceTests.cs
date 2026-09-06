using KurumiConcursos.ApplicationService.Mappers;
using KurumiConcursos.ApplicationService.Services.QuestionAppointmentServices;
using KurumiConcursos.Domain.Entities;
using KurumiConcursos.Infra.Interfaces.RepositoryContracts;
using Moq;

namespace KurumiConcursos.UnitTests.Services;

public sealed class QuestionAppointmentCommandServiceTests
{
    [Theory]
    [InlineData(1, 2)]
    [InlineData(2, 1)]
    public async Task ScheduleAsync_MustNotScheduleOnReviewDate(int reviewOffset, int expectedQuestionOffset)
    {
        var completedOn = new DateOnly(2026, 9, 6);
        QuestionAppointment? saved = null;
        var repository = new Mock<IQuestionAppointmentRepository>();
        repository.Setup(item => item.SaveAsync(It.IsAny<QuestionAppointment>()))
            .Callback<QuestionAppointment>(item => saved = item)
            .ReturnsAsync(true);
        var service = new QuestionAppointmentCommandService(repository.Object, new QuestionAppointmentMapper());

        var result = await service.ScheduleAsync(Guid.NewGuid(), 7, new SyllabusNode { Id = 10 },
            completedOn, completedOn.AddDays(reviewOffset));

        Assert.True(result);
        Assert.NotNull(saved);
        Assert.Equal(completedOn.AddDays(expectedQuestionOffset), saved.ScheduledFor);
        Assert.NotEqual(completedOn.AddDays(reviewOffset), saved.ScheduledFor);
    }
}
