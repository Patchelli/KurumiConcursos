using KurumiConcursos.ApplicationService.DataTransferObjects.StudyResourceDtos.Response;
using KurumiConcursos.Domain.ValueObjects;

namespace KurumiConcursos.ApplicationService.Interfaces.ServiceContracts;

public interface IStudyResourceQueryService
{
    Task<IList<StudyResourceResponse>> FindAllAsync(
        long journeyId,
        long? syllabusNodeId,
        long? knowledgeAreaId,
        UserCredential credential);
}