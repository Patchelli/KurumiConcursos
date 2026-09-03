namespace KurumiConcursos.ApplicationService.DataTransferObjects.FlashcardDtos.Response;

public sealed record FlashcardResponse(
    long Id,
    long JourneyId,
    long KnowledgeAreaId,
    long? SyllabusNodeId,
    string Model,
    string Type,
    string Front,
    string Back,
    bool? CorrectAnswer,
    DateOnly? NextReviewOn);