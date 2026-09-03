namespace KurumiConcursos.ApplicationService.DataTransferObjects.FlashcardDtos.Request;

public sealed record FlashcardRegisterRequest(
    long JourneyId,
    long KnowledgeAreaId,
    long? SyllabusNodeId,
    string Model,
    string Type,
    string Front,
    string Back,
    bool? CorrectAnswer);