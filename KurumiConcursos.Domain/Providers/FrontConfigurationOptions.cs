namespace KurumiConcursos.Domain.Providers;

public sealed class FrontConfigurationOptions
{
    public const string SectionName = "FrontConfiguration";
    public string Web { get; init; } = string.Empty;
    public string Mobile { get; init; } = string.Empty;
    public string[] Methods { get; init; } = [];
}
