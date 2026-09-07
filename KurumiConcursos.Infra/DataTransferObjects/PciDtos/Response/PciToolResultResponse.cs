namespace KurumiConcursos.Infra.DataTransferObjects.PciDtos.Response;

public sealed record PciToolResultResponse
{
    public bool IsError { get; init; }
    public IReadOnlyList<PciContentResponse>? Content { get; init; }
}