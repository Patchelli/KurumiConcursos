using System.Text.Json;
using KurumiConcursos.ApplicationService.DataTransferObjects.FlashcardDtos.Request;
using KurumiConcursos.ApplicationService.DataTransferObjects.FlashcardDtos.Response;
using KurumiConcursos.Domain.Enums;

namespace KurumiConcursos.ApplicationService.Services.FlashcardServices;

internal static class FlashcardReviewIntervals
{
    public static FlashcardReviewIntervalsResponse FromJson(string? json)
    {
        if (string.IsNullOrWhiteSpace(json)) return new();
        try
        {
            var intervals = JsonSerializer.Deserialize<FlashcardReviewIntervalsResponse>(json);
            return intervals is not null && IsValid(intervals) ? intervals : new();
        }
        catch (JsonException)
        {
            return new();
        }
    }

    public static FlashcardReviewIntervalsResponse FromRequest(FlashcardReviewIntervalsRequest request) =>
        new(request.AgainHours, request.HardHours, request.GoodHours, request.EasyHours);

    public static bool IsValid(FlashcardReviewIntervalsResponse intervals) =>
        new[] { intervals.AgainHours, intervals.HardHours, intervals.GoodHours, intervals.EasyHours }
            .All(hours => hours is >= 1 and <= 8760);

    public static int ForGrade(FlashcardReviewIntervalsResponse intervals, ERecallGrade grade) => grade switch
    {
        ERecallGrade.Again => intervals.AgainHours,
        ERecallGrade.Hard => intervals.HardHours,
        ERecallGrade.Good => intervals.GoodHours,
        ERecallGrade.Easy => intervals.EasyHours,
        _ => throw new ArgumentOutOfRangeException(nameof(grade))
    };
}