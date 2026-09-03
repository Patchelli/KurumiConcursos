using KurumiConcursos.ApplicationService.DataTransferObjects.FlashcardDtos.Response;
using KurumiConcursos.Domain.ValueObjects;

namespace KurumiConcursos.ApplicationService.Interfaces.ServiceContracts;

public interface IFlashcardQueryService
{
    Task<FlashcardPracticeResponse> FindPracticeAsync(
        long journeyId, long? knowledgeAreaId, long? syllabusNodeId,
        bool includeDescendants, UserCredential credential);

    Task<IList<FlashcardResponse>> FindAllAsync(
        long journeyId, long? knowledgeAreaId, long? syllabusNodeId, UserCredential credential);
}