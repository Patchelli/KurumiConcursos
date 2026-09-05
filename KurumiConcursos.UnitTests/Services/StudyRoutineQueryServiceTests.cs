using System.Linq.Expressions;
using System.Text.Json;
using KurumiConcursos.ApplicationService.DataTransferObjects.StudyRoutineDtos.Request;
using KurumiConcursos.ApplicationService.Interfaces.MapperContracts;
using KurumiConcursos.ApplicationService.Services.StudyRoutineServices;
using KurumiConcursos.Domain.Entities;
using KurumiConcursos.Domain.Enums;
using KurumiConcursos.Domain.ValueObjects;
using KurumiConcursos.Infra.Interfaces.RepositoryContracts;
using Moq;

namespace KurumiConcursos.UnitTests.Services;

public sealed class StudyRoutineQueryServiceTests
{
    [Fact]
    public async Task OverdueTopicWithoutFutureSubjectSlot_DoesNotAccumulateToday()
    {
        var userId = Guid.NewGuid();
        var from = new DateOnly(2026, 9, 5);
        var blocks = new List<StudyRoutineBlock>
        {
            Block(1, userId, 101, from.AddDays(-1), 0),
            Block(2, userId, 201, from, 1),
            Block(3, userId, 301, from.AddDays(1), 2)
        };
        var areas = new[]
        {
            Area(10, 101),
            Area(20, 201),
            Area(30, 301)
        };
        var configuration = new StudyRoutineConfigurationRequest(
            [10, 20, 30],
            new Dictionary<long, string>(),
            1,
            7,
            50,
            25,
            25,
            EveryDay(1),
            new Dictionary<long, decimal>(),
            new Dictionary<long, decimal>());

        var blockRepository = new Mock<IStudyRoutineBlockRepository>();
        blockRepository
            .Setup(repository => repository.FindAllAsync(
                It.IsAny<Expression<Func<StudyRoutineBlock, bool>>>(),
                null))
            .ReturnsAsync((Expression<Func<StudyRoutineBlock, bool>> predicate,
                Func<IQueryable<StudyRoutineBlock>, Microsoft.EntityFrameworkCore.Query.IIncludableQueryable<StudyRoutineBlock, object>>? _) =>
                blocks.Where(predicate.Compile()).ToList());
        blockRepository.Setup(repository => repository.UpdateAsync(It.IsAny<StudyRoutineBlock>()))
            .ReturnsAsync(true);

        var routineRepository = new Mock<IStudyRoutineRepository>();
        routineRepository
            .Setup(repository => repository.FindByPredicateAsync(
                It.IsAny<Expression<Func<StudyRoutine, bool>>>(),
                null,
                true))
            .ReturnsAsync(new StudyRoutine
            {
                Id = 1,
                UserId = userId,
                JourneyId = 1,
                ConfigurationJson = JsonSerializer.Serialize(configuration)
            });

        var journeyRepository = new Mock<IJourneyRepository>();
        journeyRepository
            .Setup(repository => repository.FindByIdAsync(1, userId, CancellationToken.None, true, false))
            .ReturnsAsync(new ExamJourney { Id = 1, UserId = userId, KnowledgeAreas = areas });

        var service = new StudyRoutineQueryService(
            routineRepository.Object,
            blockRepository.Object,
            journeyRepository.Object,
            Mock.Of<IStudyRoutineMapper>());

        await service.FindBlocksAsync(
            1,
            from,
            from.AddDays(10),
            new UserCredential { UserId = userId, Roles = [] });

        Assert.NotEqual(from, blocks.Single(block => block.Id == 1).ScheduledFor);
        Assert.All(
            blocks.Where(block => block.ScheduledFor >= from).GroupBy(block => block.ScheduledFor),
            day => Assert.True(day.Sum(block => block.PlannedMinutes) <= 60));
    }

    [Fact]
    public async Task OverdueTopic_UsesNextSlotOfSameSubjectAndPushesItsContent()
    {
        var userId = Guid.NewGuid();
        var from = new DateOnly(2026, 9, 5);
        var blocks = new List<StudyRoutineBlock>
        {
            Block(1, userId, 101, from.AddDays(-1), 0),
            Block(2, userId, 102, from.AddDays(2), 3),
            Block(3, userId, 201, from, 1),
            Block(4, userId, 301, from.AddDays(1), 2)
        };
        var configuration = new StudyRoutineConfigurationRequest(
            [10, 20, 30], new Dictionary<long, string>(), 1, 7, 50, 25, 25,
            EveryDay(1), new Dictionary<long, decimal>(), new Dictionary<long, decimal>());
        var blockRepository = BlockRepository(blocks);
        var routineRepository = RoutineRepository(userId, configuration);
        var journeyRepository = new Mock<IJourneyRepository>();
        journeyRepository
            .Setup(repository => repository.FindByIdAsync(1, userId, CancellationToken.None, true, false))
            .ReturnsAsync(new ExamJourney
            {
                Id = 1,
                UserId = userId,
                KnowledgeAreas = [Area(10, 101, 102), Area(20, 201), Area(30, 301)]
            });
        var service = new StudyRoutineQueryService(
            routineRepository.Object, blockRepository.Object, journeyRepository.Object,
            Mock.Of<IStudyRoutineMapper>());

        await service.FindBlocksAsync(1, from, from.AddDays(10),
            new UserCredential { UserId = userId, Roles = [] });

        Assert.Equal(from.AddDays(2), blocks.Single(block => block.Id == 1).ScheduledFor);
        Assert.True(blocks.Single(block => block.Id == 2).ScheduledFor > from.AddDays(2));
    }

    private static Mock<IStudyRoutineBlockRepository> BlockRepository(List<StudyRoutineBlock> blocks)
    {
        var repository = new Mock<IStudyRoutineBlockRepository>();
        repository.Setup(item => item.FindAllAsync(
                It.IsAny<Expression<Func<StudyRoutineBlock, bool>>>(), null))
            .ReturnsAsync((Expression<Func<StudyRoutineBlock, bool>> predicate,
                Func<IQueryable<StudyRoutineBlock>, Microsoft.EntityFrameworkCore.Query.IIncludableQueryable<StudyRoutineBlock, object>>? _) =>
                blocks.Where(predicate.Compile()).ToList());
        repository.Setup(item => item.UpdateAsync(It.IsAny<StudyRoutineBlock>())).ReturnsAsync(true);
        return repository;
    }

    private static Mock<IStudyRoutineRepository> RoutineRepository(
        Guid userId,
        StudyRoutineConfigurationRequest configuration)
    {
        var repository = new Mock<IStudyRoutineRepository>();
        repository.Setup(item => item.FindByPredicateAsync(
                It.IsAny<Expression<Func<StudyRoutine, bool>>>(), null, true))
            .ReturnsAsync(new StudyRoutine
            {
                Id = 1, UserId = userId, JourneyId = 1,
                ConfigurationJson = JsonSerializer.Serialize(configuration)
            });
        return repository;
    }

    private static StudyRoutineBlock Block(
        long id,
        Guid userId,
        long nodeId,
        DateOnly date,
        int order) => new()
    {
        Id = id,
        UserId = userId,
        JourneyId = 1,
        StudyRoutineId = 1,
        SyllabusNodeId = nodeId,
        ScheduledFor = date,
        Type = EStudyBlockType.Study,
        Status = EStudyBlockStatus.Pending,
        PlannedMinutes = 60,
        Order = order
    };

    private static KnowledgeArea Area(long id, params long[] nodeIds) => new()
    {
        Id = id,
        SyllabusNodes = nodeIds.Select(nodeId => new SyllabusNode
        {
            Id = nodeId,
            KnowledgeAreaId = id
        }).ToList()
    };

    private static Dictionary<string, decimal> EveryDay(decimal hours) => new()
    {
        ["SEG"] = hours, ["TER"] = hours, ["QUA"] = hours, ["QUI"] = hours,
        ["SEX"] = hours, ["SÁB"] = hours, ["DOM"] = hours
    };
}
