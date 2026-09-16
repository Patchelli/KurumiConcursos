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

    public void DtoUpdateToDomain(StudentProfile profile, RadarPreferencesRequest request) =>
        profile.RadarPreferencesJson = JsonSerializer.Serialize(new RadarPreferencesResponse(
            request.Region, request.State, request.Education, request.Role.Trim(), request.IncludeNational));

    public RadarPreferencesResponse DomainToPreferencesResponse(StudentProfile profile) =>
        string.IsNullOrWhiteSpace(profile.RadarPreferencesJson)
            ? new()
            : JsonSerializer.Deserialize<RadarPreferencesResponse>(profile.RadarPreferencesJson) ?? new();
}