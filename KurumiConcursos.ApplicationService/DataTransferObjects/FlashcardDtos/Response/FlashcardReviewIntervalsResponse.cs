namespace KurumiConcursos.ApplicationService.DataTransferObjects.FlashcardDtos.Response;

public sealed record FlashcardReviewIntervalsResponse(
    int AgainHours = 24,
    int HardHours = 5,
    int GoodHours = 72,
    int EasyHours = 168);
