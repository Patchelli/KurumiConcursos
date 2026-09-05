using KurumiConcursos.ApplicationService.DataTransferObjects.TimeCapsuleDtos.Response;
using KurumiConcursos.ApplicationService.Interfaces.MapperContracts;
using KurumiConcursos.ApplicationService.Interfaces.ServiceContracts;
using KurumiConcursos.Domain.ValueObjects;
using KurumiConcursos.Infra.Interfaces.RepositoryContracts;

namespace KurumiConcursos.ApplicationService.Services.TimeCapsuleServices;

public sealed class TimeCapsuleQueryService(
    ITimeCapsuleRepository repository,
    ITimeCapsuleMapper mapper) : ITimeCapsuleQueryService
{
    public async Task<IList<TimeCapsuleResponse>> FindAllAsync(long journeyId, UserCredential credential) =>
        mapper.DomainToDtoResponseList(await repository.FindAllAsync(item =>
            item.UserId == credential.UserId && item.JourneyId == journeyId));
}