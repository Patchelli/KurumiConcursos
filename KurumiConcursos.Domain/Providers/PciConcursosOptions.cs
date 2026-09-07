namespace KurumiConcursos.Domain.Providers;

public sealed class PciConcursosOptions
{
    public const string SectionName = "PciConcursos";
    public string ServerUrl { get; init; } = "https://mcp.pciconcursos.com.br/mcp";
    public int TimeoutSeconds { get; init; } = 30;
    public int CacheMinutes { get; init; } = 10;
}