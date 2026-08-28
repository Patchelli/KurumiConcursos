namespace KurumiConcursos.ApplicationService.DataTransferObjects.JourneyDtos.Request;

public sealed record SaveJourneyStructureRequest(
    SaveJourneyRequest Journey,
    IReadOnlyList<SaveKnowledgeAreaStructureRequest> KnowledgeAreas);
