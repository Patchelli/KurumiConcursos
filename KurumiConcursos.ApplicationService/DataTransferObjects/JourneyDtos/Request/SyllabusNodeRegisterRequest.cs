namespace KurumiConcursos.ApplicationService.DataTransferObjects.JourneyDtos.Request;

public sealed record SyllabusNodeRegisterRequest(
    long? Id,
    long KnowledgeAreaId,
    long? ParentId,
    string Title,
    int Order);