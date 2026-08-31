using KurumiConcursos.ApplicationService.DataTransferObjects.StudyResourceDtos.Response;
using KurumiConcursos.ApplicationService.Interfaces.MapperContracts;
using KurumiConcursos.ApplicationService.Interfaces.ServiceContracts;
using KurumiConcursos.Domain.ValueObjects;
using KurumiConcursos.Infra.Interfaces.RepositoryContracts;

namespace KurumiConcursos.ApplicationService.Services.StudyResourceServices;

public sealed class StudyResourceQueryService(
    IStudyResourceRepository studyResourceRepository,
    IStudyResourceMapper studyResourceMapper)
    : IStudyResourceQueryService
{
    public async Task<IList<StudyResourceResponse>> FindAllAsync(
        long journeyId,
        long? syllabusNodeId,
        UserCredential credential)
    {
        var resources = await studyResourceRepository.FindAllAsync(item =>
            item.UserId == credential.UserId &&
            item.JourneyId == journeyId &&
            (!syllabusNodeId.HasValue || item.SyllabusNodeId == syllabusNodeId));

        return studyResourceMapper.DomainToDtoResponseList(resources);
    }
}