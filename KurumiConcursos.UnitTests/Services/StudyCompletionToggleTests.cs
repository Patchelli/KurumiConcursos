using System.Linq.Expressions;
using KurumiConcursos.ApplicationService.DataTransferObjects.JourneyDtos.Request;
using KurumiConcursos.ApplicationService.DataTransferObjects.StudyRoutineBlockDtos.Request;
using KurumiConcursos.ApplicationService.Interfaces.MapperContracts;
using KurumiConcursos.ApplicationService.Interfaces.ServiceContracts;
using KurumiConcursos.ApplicationService.Mappers;
using KurumiConcursos.ApplicationService.Services.StudyRoutineServices;
using KurumiConcursos.ApplicationService.Services.SyllabusNodeStudyServices;
using KurumiConcursos.Domain.Entities;
using KurumiConcursos.Domain.Enums;
using KurumiConcursos.Domain.Handlers.NotificationHandler;
using KurumiConcursos.Domain.Handlers.ValidationHandler;
using KurumiConcursos.Domain.Interface;
using KurumiConcursos.Domain.ValueObjects;
using KurumiConcursos.Infra.Interfaces.RepositoryContracts;
using Microsoft.EntityFrameworkCore.Query;
using Moq;

namespace KurumiConcursos.UnitTests.Services;

public sealed class StudyCompletionToggleTests
{
    [Theory]
    [InlineData(false, 0)]
    [InlineData(true, 0)]
    [InlineData(false, 10)]
    [InlineData(true, 10)]
    public async Task UndoOrPendingClearsLocation_AndRecompletionDoesNotRestoreIt(bool throughPlan, int minutes)
    {
        var fixture = new Fixture();
        await fixture.Save(throughPlan, true, 20, studyLocation: "Escritorio");
        await fixture.Save(throughPlan, false, minutes);
        Assert.Null(fixture.Nodes[0].LastStudyLocation);
        if (minutes == 0)
            Assert.All(fixture.Nodes, node => Assert.Null(node.LastStudyLocation));
        await fixture.Save(throughPlan, true, 20);
        Assert.Null(fixture.Nodes[0].LastStudyLocation);
    }

    [Theory]
    [InlineData(EStudyProgress.NotStarted)]
    [InlineData(EStudyProgress.InProgress)]
    public void IncompleteTopicDoesNotExposeLegacyLocation(EStudyProgress progress)
    {
        var node = new SyllabusNode { Progress = progress, LastStudyLocation = "Escritorio" };
        Assert.Null(new SyllabusNodeStudyMapper().DomainToDtoResponse(node, 0, null).LastStudyLocation);
    }

    [Theory]
    [InlineData(false)]
    [InlineData(true)]
    public async Task CompletionRecordsLocation_AndBlankPreservesLastLocation(bool throughPlan)
    {
        var fixture = new Fixture();
        await fixture.Save(throughPlan, true, 20, studyLocation: "  Biblioteca  ");
        Assert.All(fixture.Nodes, node => Assert.Equal("Biblioteca", node.LastStudyLocation));
        await fixture.Save(throughPlan, true, 10, studyLocation: "  ");
        Assert.All(fixture.Nodes, node => Assert.Equal("Biblioteca", node.LastStudyLocation));
        await fixture.Save(throughPlan, true, 10, studyLocation: "Escritorio");
        Assert.All(fixture.Nodes, node => Assert.Equal("Escritorio", node.LastStudyLocation));
    }

    [Fact]
    public async Task ReviewUpdatesOnlyReviewedNodeLocation()
    {
        var fixture = new Fixture();
        await fixture.Save(true, true, 20, studyLocation: "Biblioteca");
        fixture.Block.Type = EStudyBlockType.Review;
        await fixture.Save(true, true, 10, studyLocation: "Casa");
        Assert.Equal("Casa", fixture.Nodes[0].LastStudyLocation);
        Assert.All(fixture.Nodes.Skip(1), node => Assert.Equal("Biblioteca", node.LastStudyLocation));
        Assert.Equal("Casa", new SyllabusNodeStudyMapper().DomainToDtoResponse(fixture.Nodes[0], 0, null).LastStudyLocation);
    }

    [Theory]
    [InlineData(false)]
    [InlineData(true)]
    public async Task PartialStudyDoesNotReplaceCompletionLocation(bool throughPlan)
    {
        var fixture = new Fixture();
        await fixture.Save(throughPlan, false, 10, studyLocation: "Casa");
        Assert.All(fixture.Nodes, node => Assert.Null(node.LastStudyLocation));
    }

    [Theory]
    [InlineData(false)]
    [InlineData(true)]
    public async Task CompleteAndUndoParentRepeatedly_ResetsAllDescendants(bool throughPlan)
    {
        var fixture = new Fixture();
        for (var cycle = 0; cycle < 3; cycle++)
        {
            await fixture.Save(throughPlan, true, 20);
            Assert.All(fixture.Nodes, node =>
            {
                Assert.Equal(EStudyProgress.Studied, node.Progress);
                Assert.NotNull(node.StudyStartedOn);
            });
            await fixture.Save(throughPlan, false, 0);
            // Repetir o desfazer não pode recriar uma pendência.
            await fixture.Save(throughPlan, false, 0);
            Assert.All(fixture.Nodes, AssertNotStarted);
            Assert.Empty(fixture.Sessions);
            Assert.Equal(0, fixture.Block.CompletedMinutes);
            Assert.Equal(EStudyBlockStatus.Pending, fixture.Block.Status);
        }
    }

    [Theory]
    [InlineData(false)]
    [InlineData(true)]
    public async Task SaveParentAsPending_DoesNotStartUntouchedChildren(bool throughPlan)
    {
        var fixture = new Fixture();
        await fixture.Save(throughPlan, false, 10);
        Assert.Equal(EStudyProgress.InProgress, fixture.Nodes[0].Progress);
        Assert.NotNull(fixture.Nodes[0].StudyStartedOn);
        Assert.All(fixture.Nodes.Skip(1), AssertNotStarted);
        await fixture.Save(throughPlan, false, 0, clearPending: true);
        Assert.All(fixture.Nodes, AssertNotStarted);
        Assert.Empty(fixture.Sessions);
    }

    [Fact]
    public async Task UndoOneSubtopic_DoesNotResetCompletedSibling()
    {
        var fixture = new Fixture();
        await fixture.SaveNode(2, true, 10);
        await fixture.SaveNode(3, true, 15);
        await fixture.SaveNode(3, false, 0);
        Assert.Equal(EStudyProgress.Studied, fixture.Nodes[1].Progress);
        AssertNotStarted(fixture.Nodes[2]);
        Assert.Equal(EStudyProgress.InProgress, fixture.Nodes[0].Progress);
        Assert.Equal(2L, Assert.Single(fixture.Sessions).SyllabusNodeId);
        Assert.Equal(10, fixture.Block.CompletedMinutes);
    }

    [Fact]
    public async Task SubtopicPendingRemainsPendingUntilExplicitlyCleared()
    {
        var fixture = new Fixture();
        await fixture.SaveNode(3, false, 5);
        Assert.Equal(EStudyProgress.InProgress, fixture.Nodes[2].Progress);
        Assert.Single(fixture.Sessions);
        await fixture.SaveNode(3, false, 0, clearPending: true);
        AssertNotStarted(fixture.Nodes[2]);
        AssertNotStarted(fixture.Nodes[0]);
        Assert.Empty(fixture.Sessions);
    }

    private static void AssertNotStarted(SyllabusNode node)
    {
        Assert.Equal(EStudyProgress.NotStarted, node.Progress);
        Assert.Null(node.StudyStartedOn);
        Assert.Null(node.StudiedOn);
    }

    private sealed class Fixture
    {
        private readonly UserCredential credential = new() { UserId = Guid.NewGuid(), Roles = [] };
        private readonly NotificationHandler notifications = new();
        private readonly SyllabusNodeStudyCommandService nodeService;
        private readonly StudyRoutineCommandService planService;
        public List<FocusSession> Sessions { get; } = [];
        public SyllabusNode[] Nodes { get; } =
        [
            new() { Id = 1, KnowledgeAreaId = 1, Progress = EStudyProgress.NotStarted },
            new() { Id = 2, ParentId = 1, KnowledgeAreaId = 1, Progress = EStudyProgress.NotStarted },
            new() { Id = 3, ParentId = 1, KnowledgeAreaId = 1, Progress = EStudyProgress.NotStarted },
            new() { Id = 4, ParentId = 2, KnowledgeAreaId = 1, Progress = EStudyProgress.NotStarted }
        ];
        public StudyRoutineBlock Block { get; }

        public Fixture()
        {
            Block = new() { Id = 1, JourneyId = 1, SyllabusNodeId = 1, StudyRoutineId = 1,
                UserId = credential.UserId, Type = EStudyBlockType.Study, Status = EStudyBlockStatus.Pending,
                ScheduledFor = DateOnly.FromDateTime(TimeZoneInfo.ConvertTimeBySystemTimeZoneId(DateTimeOffset.UtcNow, "America/Sao_Paulo").DateTime) };
            var journey = new ExamJourney { Id = 1, UserId = credential.UserId,
                KnowledgeAreas = [new KnowledgeArea { Id = 1, SyllabusNodes = Nodes }] };
            var journeys = new Mock<IJourneyRepository>();
            journeys.Setup(x => x.FindByIdAsync(1, credential.UserId, It.IsAny<CancellationToken>(), true, true)).ReturnsAsync(journey);
            journeys.Setup(x => x.FindNodeAsync(1, credential.UserId, It.IsAny<CancellationToken>(), false)).ReturnsAsync(Nodes[0]);
            journeys.Setup(x => x.UpdateNodeAsync(It.IsAny<SyllabusNode>())).ReturnsAsync(true);
            var blocks = new Mock<IStudyRoutineBlockRepository>();
            blocks.Setup(x => x.FindByPredicateAsync(It.IsAny<Expression<Func<StudyRoutineBlock, bool>>>(), null, false)).ReturnsAsync(Block);
            blocks.Setup(x => x.FindAllAsync(It.IsAny<Expression<Func<StudyRoutineBlock, bool>>>(), null))
                .ReturnsAsync((Expression<Func<StudyRoutineBlock, bool>> predicate, Func<IQueryable<StudyRoutineBlock>, IIncludableQueryable<StudyRoutineBlock, object>>? _) =>
                    (IList<StudyRoutineBlock>)new[] { Block }.Where(predicate.Compile()).ToList());
            blocks.Setup(x => x.UpdateAsync(It.IsAny<StudyRoutineBlock>())).ReturnsAsync(true);
            var sessions = new Mock<IFocusSessionRepository>();
            sessions.Setup(x => x.FindAllAsync(It.IsAny<Expression<Func<FocusSession, bool>>>(), null))
                .ReturnsAsync((Expression<Func<FocusSession, bool>> predicate, Func<IQueryable<FocusSession>, IIncludableQueryable<FocusSession, object>>? _) =>
                    (IList<FocusSession>)Sessions.Where(predicate.Compile()).ToList());
            sessions.Setup(x => x.SaveAsync(It.IsAny<FocusSession>())).Callback<FocusSession>(Sessions.Add).ReturnsAsync(true);
            sessions.Setup(x => x.DeleteAsync(It.IsAny<FocusSession>())).Callback<FocusSession>(session => Sessions.Remove(session)).ReturnsAsync(true);
            var reviews = new Mock<IReviewAppointmentRepository>();
            reviews.Setup(x => x.FindAllAsync(It.IsAny<Expression<Func<ReviewAppointment, bool>>>(), null)).ReturnsAsync(new List<ReviewAppointment>());
            var questions = new Mock<IQuestionAppointmentRepository>();
            questions.Setup(x => x.FindAllAsync(It.IsAny<Expression<Func<QuestionAppointment, bool>>>(), null)).ReturnsAsync(new List<QuestionAppointment>());
            var questionCommands = new Mock<IQuestionAppointmentCommandService>();
            questionCommands.Setup(x => x.SupersedePendingAsync(It.IsAny<Guid>(), It.IsAny<IReadOnlyCollection<long>>())).ReturnsAsync(true);
            questionCommands.Setup(x => x.ScheduleAsync(It.IsAny<Guid>(), It.IsAny<long>(), It.IsAny<SyllabusNode>(), It.IsAny<DateOnly>(), It.IsAny<DateOnly?>())).ReturnsAsync(true);
            var validation = new Mock<IValidate<SyllabusNode>>();
            validation.Setup(x => x.ValidationAsync(It.IsAny<SyllabusNode>())).ReturnsAsync(ValidationResponse.CreateResponse([]));
            var summaries = Mock.Of<IStudySummaryRepository>();
            var capsules = Mock.Of<ITimeCapsuleCommandService>();
            var logger = Mock.Of<ILoggerHandler>();
            nodeService = new(journeys.Object, sessions.Object, reviews.Object, questions.Object, questionCommands.Object,
                blocks.Object, summaries, capsules, new SyllabusNodeStudyMapper(), validation.Object, notifications, logger);
            planService = new(Mock.Of<IStudyRoutineRepository>(), journeys.Object, Mock.Of<IStudyRoutineMapper>(),
                blocks.Object, sessions.Object, summaries, reviews.Object, questionCommands.Object, capsules,
                Mock.Of<IValidate<StudyRoutine>>(), notifications, logger);
        }

        public async Task Save(bool throughPlan, bool completed, int minutes, bool clearPending = false, string? studyLocation = null)
        {
            if (throughPlan)
                Assert.NotNull(await planService.CompleteBlockAsync(new StudyRoutineBlockCompleteRequest(1, completed, minutes, false, null, clearPending, StudyLocation: studyLocation), credential));
            else
                await SaveNode(1, completed, minutes, clearPending, studyLocation);
            Assert.False(notifications.HasNotification());
        }

        public async Task SaveNode(long id, bool completed, int minutes, bool clearPending = false, string? studyLocation = null)
        {
            Assert.NotNull(await nodeService.SaveAsync(new SyllabusNodeStudyRequest(1, id, completed, minutes, false, null, clearPending, StudyLocation: studyLocation), credential));
            Assert.False(notifications.HasNotification());
        }
    }
}
