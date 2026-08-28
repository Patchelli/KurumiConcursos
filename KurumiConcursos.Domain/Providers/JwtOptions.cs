namespace KurumiConcursos.Domain.Providers;

public sealed class JwtOptions
{
    public const string SectionName = "Jwt";
    public string JwtKey { get; init; } = string.Empty;
    public string Issuer { get; init; } = string.Empty;
    public string Audience { get; init; } = string.Empty;
    public double DurationInMinutes { get; init; }
    public bool RequireHttpsMetadata { get; init; }
}
