using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using KurumiConcursos.Domain.Entities;
using KurumiConcursos.Infra.ORM.EntitiesMapping.Base;

namespace KurumiConcursos.Infra.ORM.EntitiesMapping;

public sealed class MemoryRecallMapping : MappingBase, IEntityTypeConfiguration<MemoryRecall>
{
    public void Configure(EntityTypeBuilder<MemoryRecall> b)
    {
        b.ToTable("memory_recall", Schema);
        MappingColumns.Base(b);
        b.Property(x => x.MemoryCardId).HasColumnName("memory_card_id");
        b.Property(x => x.Grade).HasColumnName("grade");
        b.Property(x => x.AnsweredAt).HasColumnName("answered_at");
        b.Property(x => x.PreviousIntervalDays).HasColumnName("previous_interval_days");
        b.Property(x => x.NewIntervalDays).HasColumnName("new_interval_days");
        b.HasOne(x => x.Card).WithMany(x => x.Recalls).HasForeignKey(x => x.MemoryCardId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}