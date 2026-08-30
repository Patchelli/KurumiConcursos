using KurumiConcursos.Domain.Entities;
using KurumiConcursos.Infra.ORM.EntitiesMapping.Base;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace KurumiConcursos.Infra.ORM.EntitiesMapping;

public sealed class MemoryCardMapping : MappingBase, IEntityTypeConfiguration<MemoryCard>
{
    public void Configure(EntityTypeBuilder<MemoryCard> b)
    {
        b.ToTable("memory_card", Schema);
        MappingColumns.Base(b);
        b.Property(x => x.FlashCollectionId).HasColumnName("collection_id");
        b.Property(x => x.Front).HasColumnName("front");
        b.Property(x => x.Back).HasColumnName("back");
        b.Property(x => x.NextReviewOn).HasColumnName("next_review_on");
        b.Property(x => x.IntervalDays).HasColumnName("interval_days");
        b.Property(x => x.EaseFactor).HasColumnName("ease_factor").HasPrecision(6, 3);
        b.HasOne(x => x.Collection).WithMany(x => x.Cards).HasForeignKey(x => x.FlashCollectionId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}