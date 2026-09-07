using System.Globalization;
using System.Text;
using KurumiConcursos.ApplicationService.DataTransferObjects.RadarDtos.Response;
using KurumiConcursos.Domain.ValueObjects;

namespace KurumiConcursos.ApplicationService.Services.RadarServices;

public static class RadarContestFilter
{
    public static IReadOnlyList<ContestOpportunity> Filter(IReadOnlyList<ContestOpportunity> contests,
        RadarPreferencesResponse preferences)
    {
        var today = DateOnly.FromDateTime(TimeZoneInfo
            .ConvertTimeBySystemTimeZoneId(DateTimeOffset.UtcNow, "America/Sao_Paulo").DateTime);
        var role = Normalize(preferences.Role.Trim());
        return contests
            .Where(contest => contest.RegistrationOpen)
            .Where(contest => contest.RegistrationEnd is null || contest.RegistrationEnd >= today)
            .Where(contest => contest.RegistrationStart is null || contest.RegistrationStart <= today)
            .Where(contest => contest.IsNational
                ? preferences.IncludeNational
                : (preferences.State.Length == 0 || contest.State == preferences.State)
                  && (preferences.Region.Length == 0 || Normalize(contest.Region) == preferences.Region))
            .Where(contest => preferences.Education.Length == 0 ||
                              Normalize(contest.Education).Contains(preferences.Education))
            .Where(contest => role.Length == 0 || Normalize(contest.Roles).Contains(role))
            .DistinctBy(contest => contest.Id)
            .OrderBy(contest => contest.RegistrationEnd ?? DateOnly.MaxValue)
            .ThenBy(contest => contest.Title)
            .ToArray();
    }

    private static string Normalize(string value) => new string(value.Normalize(NormalizationForm.FormD)
        .Where(character => CharUnicodeInfo.GetUnicodeCategory(character) != UnicodeCategory.NonSpacingMark)
        .ToArray()).ToLowerInvariant();
}