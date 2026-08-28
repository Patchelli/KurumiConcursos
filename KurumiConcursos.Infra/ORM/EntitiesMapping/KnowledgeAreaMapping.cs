using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using KurumiConcursos.Domain.Entities;
using KurumiConcursos.Infra.ORM.EntitiesMapping.Base;

namespace KurumiConcursos.Infra.ORM.EntitiesMapping;

public sealed class KnowledgeAreaMapping : MappingBase, IEntityTypeConfiguration<KnowledgeArea>
{
    public void Configure(EntityTypeBuilder<KnowledgeArea> b)
    {
        b.ToTable("knowledge_area", Schema);
        MappingColumns.Base(b);
        b.Property(x => x.JourneyId).HasColumnName("journey_id");
        b.Property(x => x.Title).HasColumnName("title").HasMaxLength(180);
        b.Property(x => x.Order).HasColumnName("display_order");
        b.Property(x => x.Weight).HasColumnName("weight").HasPrecision(8, 2);
        b.Property(x => x.ExpectedQuestions).HasColumnName("expected_questions");
        b.HasOne(x => x.Journey).WithMany(x => x.KnowledgeAreas).HasForeignKey(x => x.JourneyId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}