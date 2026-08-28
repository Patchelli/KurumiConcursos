namespace KurumiConcursos.ApplicationService.DataTransferObjects.PersonalDataDtos.Request;

public sealed record PersonalDataUpdateRequest
{
    public string? FullName { get; init; }
    public string? Document { get; init; }
    public string? Phone { get; init; }
}
