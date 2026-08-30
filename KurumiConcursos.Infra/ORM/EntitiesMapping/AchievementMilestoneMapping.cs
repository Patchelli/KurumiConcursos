using KurumiConcursos.Domain.Entities;
using KurumiConcursos.Infra.ORM.EntitiesMapping.Base;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace KurumiConcursos.Infra.ORM.EntitiesMapping;

public sealed class AchievementMilestoneMapping : MappingBase, IEntityTypeConfiguration<AchievementMilestone>
{
    public void Configure(EntityTypeBuilder<AchievementMilestone> b)
    {
        b.ToTable("achievement_milestone", Schema);
        MappingColumns.Base(b);
        b.Property(x => x.UserId).HasColumnName("user_id");
        b.Property(x => x.JourneyId).HasColumnName("journey_id");
        b.Property(x => x.Code).HasColumnName("code").HasMaxLength(80);
        b.Property(x => x.Title).HasColumnName("title").HasMaxLength(180);
        b.Property(x => x.AchievedAt).HasColumnName("achieved_at");
        b.Property(x => x.MetricsJson).HasColumnName("metrics_json").HasColumnType("jsonb");
    }
}