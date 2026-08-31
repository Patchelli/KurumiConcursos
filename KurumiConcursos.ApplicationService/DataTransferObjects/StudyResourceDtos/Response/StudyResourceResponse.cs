using KurumiConcursos.Domain.Enums;

namespace KurumiConcursos.ApplicationService.DataTransferObjects.StudyResourceDtos.Response;

public sealed record StudyResourceResponse(
    long Id,
    long JourneyId,
    long? KnowledgeAreaId,
    long? SyllabusNodeId,
    string Title,
    string Url,
    EResourceKind Kind);