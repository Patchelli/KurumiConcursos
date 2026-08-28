using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using KurumiConcursos.Domain.Entities;
using KurumiConcursos.Infra.ORM.EntitiesMapping.Base;

namespace KurumiConcursos.Infra.ORM.EntitiesMapping;

public sealed class StudyResourceMapping : MappingBase, IEntityTypeConfiguration<StudyResource>
{
    public void Configure(EntityTypeBuilder<StudyResource> b)
    {
        b.ToTable("study_resource", Schema);
        MappingColumns.Base(b);
        b.Property(x => x.AccountId).HasColumnName("account_id");
        b.Property(x => x.JourneyId).HasColumnName("journey_id");
        b.Property(x => x.KnowledgeAreaId).HasColumnName("knowledge_area_id");
        b.Property(x => x.SyllabusNodeId).HasColumnName("syllabus_node_id");
        b.Property(x => x.Title).HasColumnName("title").HasMaxLength(180);
        b.Property(x => x.Url).HasColumnName("url");
        b.Property(x => x.Kind).HasColumnName("kind");
    }
}