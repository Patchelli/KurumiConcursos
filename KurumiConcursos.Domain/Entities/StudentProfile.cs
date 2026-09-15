using KurumiConcursos.Domain.Entities.Base;

namespace KurumiConcursos.Domain.Entities;

public sealed class StudentProfile : EntityBase
{
    public Guid UserId { get; set; }
    public string? FlashcardIntervalsJson { get; set; }
    public string? RadarPreferencesJson { get; set; }
    public User User { get; set; } = null!;
    public ICollection<ExamJourney> ExamJourneys { get; set; } = [];
}
