namespace KurumiConcursos.Domain.Providers;

public sealed class PrivateMaterialsAccessOptions
{
    public const string SectionName = "PrivateMaterials";
    public string AllowedUsers { get; init; } = string.Empty;

    public IReadOnlySet<string> Emails => AllowedUsers
        .Split(',', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries)
        .Select(email => email.ToUpperInvariant()).ToHashSet(StringComparer.Ordinal);
}