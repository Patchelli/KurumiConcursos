namespace KurumiConcursos.ApplicationService.DataTransferObjects.FlashcardDtos.Response;

public sealed record FlashcardPracticeResponse(
    int TotalCards,
    int ReviewCards,
    int NewCards,
    int CorrectToday,
    IList<FlashcardResponse> Cards);