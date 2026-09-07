using System.Text.Json.Serialization;

namespace KurumiConcursos.Infra.DataTransferObjects.PciDtos.Request;

public sealed record PciToolCallRequest
{
    [JsonPropertyName("jsonrpc")] public string JsonRpc { get; init; } = "2.0";
    public int Id { get; init; } = 1;
    public string Method { get; init; } = "tools/call";
    [JsonPropertyName("params")] public PciToolCallParameters Parameters { get; init; } = new();
}