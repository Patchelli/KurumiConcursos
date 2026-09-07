namespace KurumiConcursos.ApplicationService.DataTransferObjects.RadarDtos.Response;

public sealed record RadarPreferencesResponse(
    string Region = "",
    string State = "",
    string Education = "",
    string Role = "",
    bool IncludeNational = true);