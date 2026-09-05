using KurumiConcursos.ApplicationService.DataTransferObjects.PracticeEntryDtos;
using KurumiConcursos.Domain.ValueObjects;

namespace KurumiConcursos.ApplicationService.Interfaces.ServiceContracts;

public interface IPracticeEntryService
{
    Task<IList<PracticeEntryResponse>> FindAllAsync(long journeyId, long knowledgeAreaId, long? syllabusNodeId,
        UserCredential c);

    Task<PracticeEntryResponse?> SaveAsync(long? id, PracticeEntrySaveRequest r, UserCredential c);
    Task<bool> DeleteAsync(long id, UserCredential c);
}