using KurumiConcursos.Domain.Entities;
using KurumiConcursos.Infra.ORM.EntitiesMapping.Base;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace KurumiConcursos.Infra.ORM.EntitiesMapping;

public sealed class FlashCollectionMapping : MappingBase, IEntityTypeConfiguration<FlashCollection>
{
    public void Configure(EntityTypeBuilder<FlashCollection> b)
    {
        b.ToTable("flash_collection", Schema);
        MappingColumns.Base(b);
        b.Property(x => x.UserId).HasColumnName("user_id");
        b.Property(x => x.JourneyId).HasColumnName("journey_id");
        b.Property(x => x.KnowledgeAreaId).HasColumnName("knowledge_area_id");
        b.Property(x => x.SyllabusNodeId).HasColumnName("syllabus_node_id");
        b.Property(x => x.ParentId).HasColumnName("parent_id");
        b.Property(x => x.Title).HasColumnName("title").HasMaxLength(180);
        b.Property(x => x.Order).HasColumnName("display_order");
        b.HasIndex(x => new { x.UserId, x.JourneyId, x.KnowledgeAreaId, x.SyllabusNodeId });
    }
}