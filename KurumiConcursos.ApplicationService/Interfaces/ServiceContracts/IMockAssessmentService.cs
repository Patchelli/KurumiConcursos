using KurumiConcursos.ApplicationService.DataTransferObjects.MockAssessmentDtos.Request;
using KurumiConcursos.ApplicationService.DataTransferObjects.MockAssessmentDtos.Response;
using KurumiConcursos.Domain.ValueObjects;

namespace KurumiConcursos.ApplicationService.Interfaces.ServiceContracts;

public interface IMockAssessmentService
{
    Task<IList<MockAssessmentResponse>> FindAllAsync(long journeyId, UserCredential credential);
    Task<MockAssessmentResponse?> RegisterAsync(MockAssessmentSaveRequest request, UserCredential credential);
    Task<MockAssessmentResponse?> UpdateAsync(long id, MockAssessmentSaveRequest request, UserCredential credential);
    Task<bool> DeleteAsync(long id, UserCredential credential);
}