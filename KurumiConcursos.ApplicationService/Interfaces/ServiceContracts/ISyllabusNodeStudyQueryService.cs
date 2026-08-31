using KurumiConcursos.ApplicationService.DataTransferObjects.JourneyDtos.Response;
using KurumiConcursos.Domain.ValueObjects;

namespace KurumiConcursos.ApplicationService.Interfaces.ServiceContracts;

public interface ISyllabusNodeStudyQueryService
{
    Task<IList<SyllabusNodeStudyResponse>> FindAllAsync(long journeyId, UserCredential credential);
}