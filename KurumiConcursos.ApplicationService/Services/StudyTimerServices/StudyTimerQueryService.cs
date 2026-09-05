using KurumiConcursos.ApplicationService.DataTransferObjects.StudyTimerDtos.Response;
using KurumiConcursos.ApplicationService.Interfaces.MapperContracts;
using KurumiConcursos.ApplicationService.Interfaces.ServiceContracts;
using KurumiConcursos.Domain.ValueObjects;
using KurumiConcursos.Infra.Interfaces.RepositoryContracts;

namespace KurumiConcursos.ApplicationService.Services.StudyTimerServices;

public sealed class StudyTimerQueryService(IStudyTimerSessionRepository repository, IStudyTimerMapper mapper)
    : IStudyTimerQueryService
{
    public async Task<StudyTimerResponse?> FindActiveAsync(UserCredential credential)
    {
        var session = await repository.FindByUserAsync(credential.UserId);
        return session is null ? null : mapper.DomainToDtoResponse(session, DateTimeOffset.UtcNow);
    }
}
