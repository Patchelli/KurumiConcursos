namespace KurumiConcursos.Domain.Providers;

public sealed class CorsConfigurationOptions
{
    public const string SectionName = "CorsConfiguration";
    public string Web { get; init; } = string.Empty;
    public string Mobile { get; init; } = string.Empty;
    public string[] Methods { get; init; } = [];
}
