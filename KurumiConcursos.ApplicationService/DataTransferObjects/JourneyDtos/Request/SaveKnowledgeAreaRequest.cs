namespace KurumiConcursos.ApplicationService.DataTransferObjects.JourneyDtos.Request;

public sealed record SaveKnowledgeAreaRequest(
    long? Id,
    long JourneyId,
    string Title,
    int Order,
    decimal? Weight,
    int? ExpectedQuestions);
