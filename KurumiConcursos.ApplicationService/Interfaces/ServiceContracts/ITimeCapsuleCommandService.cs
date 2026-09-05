using KurumiConcursos.ApplicationService.DataTransferObjects.TimeCapsuleDtos.Request;
using KurumiConcursos.ApplicationService.DataTransferObjects.TimeCapsuleDtos.Response;
using KurumiConcursos.Domain.ValueObjects;

namespace KurumiConcursos.ApplicationService.Interfaces.ServiceContracts;

public interface ITimeCapsuleCommandService
{
    Task<TimeCapsuleResponse?> RegisterAsync(TimeCapsuleRegisterRequest request, UserCredential credential);
    Task<TimeCapsuleResponse?> UpdateAsync(long id, TimeCapsuleUpdateRequest request, UserCredential credential);
    Task<TimeCapsuleResponse?> OpenAsync(long id, UserCredential credential);
    Task<bool> DeleteAsync(long id, UserCredential credential);
    Task EvaluateDateTriggersAsync();
    Task EvaluateNonDateTriggersAsync();
    Task EvaluateJourneyTriggersAsync(Guid userId, long journeyId);
}