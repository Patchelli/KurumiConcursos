using KurumiConcursos.Domain.Entities;
using KurumiConcursos.Infra.ORM.EntitiesMapping.Base;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace KurumiConcursos.Infra.ORM.EntitiesMapping;

public sealed class TopicMaterialMapping : MappingBase, IEntityTypeConfiguration<TopicMaterial>
{
    public void Configure(EntityTypeBuilder<TopicMaterial> builder)
    {
        builder.ToTable("topic_material", Schema);
        MappingColumns.Base(builder);
        builder.Property(item => item.SyllabusNodeId).HasColumnName("syllabus_node_id");
        builder.Property(item => item.KnowledgeAreaId).HasColumnName("knowledge_area_id");
        builder.Property(item => item.Name).HasColumnName("name").HasMaxLength(260);
        builder.Property(item => item.NextcloudPath).HasColumnName("nextcloud_path").HasMaxLength(2048);
        builder.Property(item => item.MimeType).HasColumnName("mime_type").HasMaxLength(100);
        builder.HasOne(item => item.SyllabusNode).WithMany().HasForeignKey(item => item.SyllabusNodeId)
            .OnDelete(DeleteBehavior.Cascade);
        builder.HasOne(item => item.KnowledgeArea).WithMany().HasForeignKey(item => item.KnowledgeAreaId)
            .OnDelete(DeleteBehavior.Cascade);
        builder.HasIndex(item => item.SyllabusNodeId);
        builder.HasIndex(item => item.KnowledgeAreaId);
    }
}
