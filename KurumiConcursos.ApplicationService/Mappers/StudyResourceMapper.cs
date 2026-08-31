using KurumiConcursos.ApplicationService.DataTransferObjects.StudyResourceDtos.Request;
using KurumiConcursos.ApplicationService.DataTransferObjects.StudyResourceDtos.Response;
using KurumiConcursos.ApplicationService.Interfaces.MapperContracts;
using KurumiConcursos.Domain.Entities;

namespace KurumiConcursos.ApplicationService.Mappers;

public sealed class StudyResourceMapper : IStudyResourceMapper
{
    public StudyResource DtoRegisterToDomain(Guid userId, StudyResourceRegisterRequest dto) => new()
    {
        UserId = userId, JourneyId = dto.JourneyId, KnowledgeAreaId = dto.KnowledgeAreaId,
        SyllabusNodeId = dto.SyllabusNodeId, Title = dto.Title.Trim(), Url = dto.Url.Trim(), Kind = dto.Kind
    };

    public StudyResourceResponse DomainToDtoResponse(StudyResource e) => new(e.Id, e.JourneyId, e.KnowledgeAreaId,
        e.SyllabusNodeId, e.Title, e.Url, e.Kind);

    public IList<StudyResourceResponse> DomainToDtoResponseList(IList<StudyResource> entities) =>
        entities.Select(DomainToDtoResponse).ToList();
}