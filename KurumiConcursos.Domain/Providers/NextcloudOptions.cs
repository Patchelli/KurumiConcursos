namespace KurumiConcursos.Domain.Providers;

public sealed class NextcloudOptions
{
    public const string SectionName = "Nextcloud";
    public string Url { get; init; } = string.Empty;
    public string User { get; init; } = string.Empty;
    public string AppPassword { get; init; } = string.Empty;
    public string RootPath { get; init; } = "/";
    public int TimeoutSeconds { get; init; } = 120;
}