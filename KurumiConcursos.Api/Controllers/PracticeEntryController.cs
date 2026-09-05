using KurumiConcursos.Api.Extensions;
using KurumiConcursos.ApplicationService.DataTransferObjects.PracticeEntryDtos;
using KurumiConcursos.ApplicationService.Interfaces.ServiceContracts;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace KurumiConcursos.Api.Controllers;

[ApiController, Route("api/[controller]"), Authorize]
public sealed class PracticeEntryController(IPracticeEntryService service) : ControllerBase
{
    [HttpGet("list")]
    public Task<IList<PracticeEntryResponse>> List(long journeyId, long knowledgeAreaId, long? syllabusNodeId) =>
        service.FindAllAsync(journeyId, knowledgeAreaId, syllabusNodeId, User.GetUserCredential());

    [HttpPost("register")]
    public Task<PracticeEntryResponse?> Register(PracticeEntrySaveRequest r) =>
        service.SaveAsync(null, r, User.GetUserCredential());

    [HttpPut("{id:long}")]
    public Task<PracticeEntryResponse?> Update(long id, PracticeEntrySaveRequest r) =>
        service.SaveAsync(id, r, User.GetUserCredential());

    [HttpDelete("{id:long}")]
    public Task<bool> Delete(long id) => service.DeleteAsync(id, User.GetUserCredential());
}