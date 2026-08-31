using KurumiConcursos.ApplicationService.DataTransferObjects.StudyResourceDtos.Request;
using KurumiConcursos.ApplicationService.DataTransferObjects.StudyResourceDtos.Response;
using KurumiConcursos.Domain.Entities;

namespace KurumiConcursos.ApplicationService.Interfaces.MapperContracts;

public interface IStudyResourceMapper
{
    StudyResource DtoRegisterToDomain(Guid userId, StudyResourceRegisterRequest dto);
    StudyResourceResponse DomainToDtoResponse(StudyResource entity);
    IList<StudyResourceResponse> DomainToDtoResponseList(IList<StudyResource> entities);
}