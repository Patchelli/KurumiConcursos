namespace KurumiConcursos.ApplicationService.DataTransferObjects.FlashcardDtos.Request;

public sealed record FlashcardUpdateRequest(
    long Id,
    string Model,
    string Type,
    string Front,
    string Back,
    bool? CorrectAnswer);