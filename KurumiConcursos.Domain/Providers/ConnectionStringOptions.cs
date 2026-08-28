namespace KurumiConcursos.Domain.Providers;

public sealed class ConnectionStringOptions
{
    public const string SectionName = "ConnectionStrings";
    public string DefaultConnection { get; init; } = string.Empty;
}
