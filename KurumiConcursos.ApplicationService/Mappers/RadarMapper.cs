using System.Text.Json;
using KurumiConcursos.ApplicationService.DataTransferObjects.RadarDtos.Request;
using KurumiConcursos.ApplicationService.DataTransferObjects.RadarDtos.Response;
using KurumiConcursos.ApplicationService.Interfaces.MapperContracts;
using KurumiConcursos.Domain.Entities;
using KurumiConcursos.Domain.ValueObjects;

namespace KurumiConcursos.ApplicationService.Mappers;

public sealed class RadarMapper : IRadarMapper
{
    public RadarResultResponse DomainToResultResponse(ContestFeed feed)
    {
        var today = DateOnly.FromDateTime(TimeZoneInfo
            .ConvertTimeBySystemTimeZoneId(DateTimeOffset.UtcNow, "America/Sao_Paulo").DateTime);
        return new(feed.Contests.Select(contest => new RadarContestResponse(
            contest.Id, contest.Title, contest.Roles, contest.Education,
            contest.IsNational ? "Nacional" : contest.State, contest.VacanciesSalary,
            contest.RegistrationEnd?.ToString("yyyy-MM-dd"),
            contest.RegistrationEnd?.DayNumber - today.DayNumber, contest.Url)).ToArray(), feed.UpdatedAt);
    }

    public void DtoUpdateToDomain(User user, RadarPreferencesRequest request) =>
        user.RadarPreferencesJson = JsonSerializer.Serialize(new RadarPreferencesResponse(
            request.Region, request.State, request.Education, request.Role.Trim(), request.IncludeNational));

    public RadarPreferencesResponse DomainToPreferencesResponse(User user) =>
        string.IsNullOrWhiteSpace(user.RadarPreferencesJson)
            ? new()
            : JsonSerializer.Deserialize<RadarPreferencesResponse>(user.RadarPreferencesJson) ?? new();
}