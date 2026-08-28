namespace KurumiConcursos.ApplicationService.DataTransferObjects.JourneyDtos.Request;

public sealed record SaveSyllabusNodeRequest(long? Id, long KnowledgeAreaId, long? ParentId, string Title, int Order);
