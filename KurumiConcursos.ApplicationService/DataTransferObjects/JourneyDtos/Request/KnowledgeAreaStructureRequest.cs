namespace KurumiConcursos.ApplicationService.DataTransferObjects.JourneyDtos.Request;

public sealed record KnowledgeAreaStructureRequest(
    string Title,
    int Order,
    IReadOnlyList<SyllabusNodeStructureRequest> Nodes);