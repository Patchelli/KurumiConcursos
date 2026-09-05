using KurumiConcursos.Domain.Entities;
using KurumiConcursos.Infra.ORM.EntitiesMapping.Base;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace KurumiConcursos.Infra.ORM.EntitiesMapping;

public sealed class TimeCapsuleMapping : MappingBase, IEntityTypeConfiguration<TimeCapsule>
{
    public void Configure(EntityTypeBuilder<TimeCapsule> b)
    {
        b.ToTable("time_capsule", Schema);
        MappingColumns.Base(b);
        b.Property(x => x.UserId).HasColumnName("user_id");
        b.Property(x => x.JourneyId).HasColumnName("journey_id");
        b.Property(x => x.Title).HasColumnName("title").HasMaxLength(100).IsRequired();
        b.Property(x => x.Message).HasColumnName("message").HasMaxLength(10000).IsRequired();
        b.Property(x => x.VideoUrl).HasColumnName("video_url").HasMaxLength(1000);
        b.Property(x => x.Status).HasColumnName("status");
        b.Property(x => x.TriggerType).HasColumnName("trigger_type");
        b.Property(x => x.TriggerReferenceId).HasColumnName("trigger_reference_id");
        b.Property(x => x.TriggerReferenceLabel).HasColumnName("trigger_reference_label").HasMaxLength(300);
        b.Property(x => x.TriggerValue).HasColumnName("trigger_value").HasPrecision(10, 2);
        b.Property(x => x.ScheduledAt).HasColumnName("scheduled_at");
        b.Property(x => x.DeliveredAt).HasColumnName("delivered_at");
        b.Property(x => x.OpenedAt).HasColumnName("opened_at");
        b.HasIndex(x => new { x.Status, x.ScheduledAt });
        b.HasIndex(x => new { x.UserId, x.JourneyId });
    }
}