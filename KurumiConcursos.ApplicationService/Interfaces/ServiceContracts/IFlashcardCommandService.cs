using KurumiConcursos.ApplicationService.DataTransferObjects.FlashcardDtos.Request;
using KurumiConcursos.ApplicationService.DataTransferObjects.FlashcardDtos.Response;
using KurumiConcursos.Domain.ValueObjects;

namespace KurumiConcursos.ApplicationService.Interfaces.ServiceContracts;

public interface IFlashcardCommandService
{
    Task<FlashcardResponse?> RegisterAsync(FlashcardRegisterRequest request, UserCredential credential);
    Task<FlashcardResponse?> RecallAsync(FlashcardRecallRequest request, UserCredential credential);
    Task<FlashcardResponse?> UpdateAsync(FlashcardUpdateRequest request, UserCredential credential);
    Task<bool> DeleteAsync(long id, UserCredential credential);
}