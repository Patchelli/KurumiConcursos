namespace KurumiConcursos.ApplicationService.DataTransferObjects.PersonalDataDtos.Response;

public sealed record PersonalDataResponse
{
    public string? FullName { get; init; }
    public string? Document { get; init; }
    public string? Phone { get; init; }
}
