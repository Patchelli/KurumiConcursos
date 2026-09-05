using KurumiConcursos.Api.Extensions;
using KurumiConcursos.ApplicationService.DataTransferObjects.MockAssessmentDtos.Request;
using KurumiConcursos.ApplicationService.DataTransferObjects.MockAssessmentDtos.Response;
using KurumiConcursos.ApplicationService.Interfaces.ServiceContracts;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace KurumiConcursos.Api.Controllers;

[ApiController, Route("api/[controller]"), Authorize]
public sealed class MockAssessmentController(IMockAssessmentService service) : ControllerBase
{
    [HttpGet("list")]
    public Task<IList<MockAssessmentResponse>> List(long journeyId) =>
        service.FindAllAsync(journeyId, User.GetUserCredential());

    [HttpPost("register")]
    public Task<MockAssessmentResponse?> Register(MockAssessmentSaveRequest request) =>
        service.RegisterAsync(request, User.GetUserCredential());

    [HttpPut("{id:long}")]
    public Task<MockAssessmentResponse?> Update(long id, MockAssessmentSaveRequest request) =>
        service.UpdateAsync(id, request, User.GetUserCredential());

    [HttpDelete("{id:long}")]
    public Task<bool> Delete(long id) => service.DeleteAsync(id, User.GetUserCredential());
}