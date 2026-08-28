using KurumiConcursos.Domain.Entities.Base;

namespace KurumiConcursos.Domain.Entities;

public sealed class AchievementMilestone : EntityBase
{
    public Guid AccountId { get; set; }
    public long JourneyId { get; set; }
    public string Code { get; set; } = string.Empty;
    public string Title { get; set; } = string.Empty;
    public DateTimeOffset AchievedAt { get; set; }
    public string MetricsJson { get; set; } = "{}";
}
