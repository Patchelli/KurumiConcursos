namespace KurumiConcursos.Infra.DataTransferObjects.PciDtos.Response;

public sealed record PciRpcErrorResponse(int Code, string? Message);