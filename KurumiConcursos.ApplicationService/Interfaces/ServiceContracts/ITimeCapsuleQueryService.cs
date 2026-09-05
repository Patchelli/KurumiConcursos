using KurumiConcursos.ApplicationService.DataTransferObjects.TimeCapsuleDtos.Response;
using KurumiConcursos.Domain.ValueObjects;

namespace KurumiConcursos.ApplicationService.Interfaces.ServiceContracts;

public interface ITimeCapsuleQueryService
{
    Task<IList<TimeCapsuleResponse>> FindAllAsync(long journeyId, UserCredential credential);
}