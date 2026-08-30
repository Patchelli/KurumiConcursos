namespace KurumiConcursos.ApplicationService.DataTransferObjects.JourneyDtos.Request;

public sealed record KnowledgeAreaRegisterRequest(
    long? Id,
    long JourneyId,
    string Title,
    int Order,
    decimal? Weight,
    int? ExpectedQuestions);