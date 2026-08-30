namespace KurumiConcursos.Domain.Providers;

public sealed class EnvironmentConfigurationOptions
{
    public const string SectionName = "EnvironmentConfiguration";
    public bool ActiveSwagger { get; init; }
}