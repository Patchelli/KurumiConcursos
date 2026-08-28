using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using KurumiConcursos.Domain.Entities;
using KurumiConcursos.Infra.ORM.EntitiesMapping.Base;

namespace KurumiConcursos.Infra.ORM.EntitiesMapping;

public sealed class FlashCollectionMapping : MappingBase, IEntityTypeConfiguration<FlashCollection>
{
    public void Configure(EntityTypeBuilder<FlashCollection> b)
    {
        b.ToTable("flash_collection", Schema);
        MappingColumns.Base(b);
        b.Property(x => x.UserId).HasColumnName("user_id");
        b.Property(x => x.JourneyId).HasColumnName("journey_id");
        b.Property(x => x.ParentId).HasColumnName("parent_id");
        b.Property(x => x.Title).HasColumnName("title").HasMaxLength(180);
        b.Property(x => x.Order).HasColumnName("display_order");
    }
}