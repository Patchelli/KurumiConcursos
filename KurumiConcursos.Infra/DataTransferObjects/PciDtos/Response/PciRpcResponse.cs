namespace KurumiConcursos.Infra.DataTransferObjects.PciDtos.Response;

public sealed record PciRpcResponse
{
    public int? Id { get; init; }
    public PciRpcErrorResponse? Error { get; init; }
    public PciToolResultResponse? Result { get; init; }
}