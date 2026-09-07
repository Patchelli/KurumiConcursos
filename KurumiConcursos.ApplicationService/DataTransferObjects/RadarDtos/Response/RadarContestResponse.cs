namespace KurumiConcursos.ApplicationService.DataTransferObjects.RadarDtos.Response;

public sealed record RadarContestResponse(
    int Id,
    string Title,
    string Roles,
    string Education,
    string Location,
    string VacanciesSalary,
    string? Deadline,
    int? DaysRemaining,
    string? Url);