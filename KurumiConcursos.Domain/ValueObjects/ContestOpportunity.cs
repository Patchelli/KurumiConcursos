namespace KurumiConcursos.Domain.ValueObjects;

public sealed record ContestOpportunity(
    int Id,
    string Title,
    string Roles,
    string Education,
    string Region,
    string State,
    string VacanciesSalary,
    bool RegistrationOpen,
    DateOnly? RegistrationStart,
    DateOnly? RegistrationEnd,
    string? Url)
{
    public bool IsNational => Region.Equals("NACIONAL", StringComparison.OrdinalIgnoreCase)
                              || Region.Equals("BRASIL", StringComparison.OrdinalIgnoreCase)
                              || State.Equals("BR", StringComparison.OrdinalIgnoreCase)
                              || State.Equals("NACIONAL", StringComparison.OrdinalIgnoreCase);
}