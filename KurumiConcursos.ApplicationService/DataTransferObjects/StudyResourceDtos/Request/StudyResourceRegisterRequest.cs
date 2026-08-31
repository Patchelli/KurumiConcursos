using KurumiConcursos.Domain.Enums;

namespace KurumiConcursos.ApplicationService.DataTransferObjects.StudyResourceDtos.Request;

public sealed record StudyResourceRegisterRequest(
    long JourneyId,
    long? KnowledgeAreaId,
    long? SyllabusNodeId,
    string Title,
    string Url,
    EResourceKind Kind);