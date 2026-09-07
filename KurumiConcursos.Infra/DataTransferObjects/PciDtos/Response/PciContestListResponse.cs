namespace KurumiConcursos.Infra.DataTransferObjects.PciDtos.Response;

public sealed record PciContestListResponse
{
    public required IReadOnlyList<PciContestResponse> Data { get; init; }
}