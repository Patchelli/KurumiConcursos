namespace KurumiConcursos.Infra.DataTransferObjects.PciDtos.Request;

public sealed record PciToolCallParameters
{
    public string Name { get; init; } = "listar_concursos";
    public IReadOnlyDictionary<string, string> Arguments { get; init; } = new Dictionary<string, string>();
}