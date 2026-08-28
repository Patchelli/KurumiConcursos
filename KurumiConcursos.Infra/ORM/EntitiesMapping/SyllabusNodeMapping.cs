using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using KurumiConcursos.Domain.Entities;
using KurumiConcursos.Infra.ORM.EntitiesMapping.Base;

namespace KurumiConcursos.Infra.ORM.EntitiesMapping;

public sealed class SyllabusNodeMapping : MappingBase, IEntityTypeConfiguration<SyllabusNode>
{
    public void Configure(EntityTypeBuilder<SyllabusNode> b)
    {
        b.ToTable("syllabus_node", Schema);
        MappingColumns.Base(b);
        b.Property(x => x.KnowledgeAreaId).HasColumnName("knowledge_area_id");
        b.Property(x => x.ParentId).HasColumnName("parent_id");
        b.Property(x => x.Title).HasColumnName("title").HasMaxLength(300);
        b.Property(x => x.Order).HasColumnName("display_order");
        b.Property(x => x.Progress).HasColumnName("progress");
        b.Property(x => x.StudyStartedOn).HasColumnName("study_started_on");
        b.Property(x => x.StudiedOn).HasColumnName("studied_on");
        b.HasOne(x => x.KnowledgeArea).WithMany(x => x.SyllabusNodes).HasForeignKey(x => x.KnowledgeAreaId)
            .OnDelete(DeleteBehavior.Cascade);
        b.HasOne(x => x.Parent).WithMany(x => x.Children).HasForeignKey(x => x.ParentId)
            .OnDelete(DeleteBehavior.Restrict);
    }
}