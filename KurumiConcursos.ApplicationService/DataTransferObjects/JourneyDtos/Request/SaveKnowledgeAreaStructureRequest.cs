namespace KurumiConcursos.ApplicationService.DataTransferObjects.JourneyDtos.Request;

public sealed record SaveKnowledgeAreaStructureRequest(
    string Title,
    int Order,
    IReadOnlyList<SaveSyllabusNodeStructureRequest> Nodes);
