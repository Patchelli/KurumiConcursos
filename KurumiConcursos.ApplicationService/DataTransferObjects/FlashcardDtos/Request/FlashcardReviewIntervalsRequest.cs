using System.ComponentModel.DataAnnotations;

namespace KurumiConcursos.ApplicationService.DataTransferObjects.FlashcardDtos.Request;

public sealed record FlashcardReviewIntervalsRequest
{
    [Range(1, 8760)] public int AgainHours { get; init; } = 24;
    [Range(1, 8760)] public int HardHours { get; init; } = 5;
    [Range(1, 8760)] public int GoodHours { get; init; } = 72;
    [Range(1, 8760)] public int EasyHours { get; init; } = 168;
}
