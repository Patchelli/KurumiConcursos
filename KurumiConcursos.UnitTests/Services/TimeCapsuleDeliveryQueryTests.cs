using System.Linq.Expressions;
using KurumiConcursos.ApplicationService.Mappers;
using KurumiConcursos.ApplicationService.Services.TimeCapsuleServices;
using KurumiConcursos.Domain.Entities;
using KurumiConcursos.Domain.Enums;
using KurumiConcursos.Domain.ValueObjects;
using KurumiConcursos.Infra.Interfaces.RepositoryContracts;
using Moq;

namespace KurumiConcursos.UnitTests.Services;

public sealed class TimeCapsuleDeliveryQueryTests
{
    [Fact]
    public async Task DeliveryInboxIncludesAllUserJourneys_WithoutOtherUsersOrUndeliveredCapsules()
    {
        var userId = Guid.NewGuid();
        var capsules = new List<TimeCapsule>
        {
            new() { Id = 1, UserId = userId, JourneyId = 10, Status = ECapsuleStatus.Delivered },
            new() { Id = 2, UserId = userId, JourneyId = 20, Status = ECapsuleStatus.Delivered },
            new() { Id = 3, UserId = Guid.NewGuid(), JourneyId = 30, Status = ECapsuleStatus.Delivered },
            new() { Id = 4, UserId = userId, Status = ECapsuleStatus.Scheduled },
            new() { Id = 5, UserId = userId, Status = ECapsuleStatus.Opened }
        };
        var repository = new Mock<ITimeCapsuleRepository>();
        repository.Setup(x => x.FindAllAsync(It.IsAny<Expression<Func<TimeCapsule, bool>>>(), false))
            .ReturnsAsync((Expression<Func<TimeCapsule, bool>> predicate, bool _) =>
                (IList<TimeCapsule>)capsules.Where(predicate.Compile()).ToList());
        var service = new TimeCapsuleQueryService(repository.Object, new TimeCapsuleMapper());

        var delivered = await service.FindDeliveredAsync(new UserCredential { UserId = userId, Roles = [] });

        Assert.Equal(new long[] { 1, 2 }, delivered.Select(item => item.Id));
    }
}
