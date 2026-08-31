using KurumiConcursos.ApplicationService.DataTransferObjects.JourneyDtos.Request;
using KurumiConcursos.ApplicationService.DataTransferObjects.JourneyDtos.Response;
using KurumiConcursos.Domain.ValueObjects;

namespace KurumiConcursos.ApplicationService.Interfaces.ServiceContracts;

public interface ISyllabusNodeStudyCommandService
{
    Task<SyllabusNodeStudyResponse?> SaveAsync(SyllabusNodeStudyRequest request, UserCredential credential);
}