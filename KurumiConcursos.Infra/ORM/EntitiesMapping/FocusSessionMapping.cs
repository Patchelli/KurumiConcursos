using KurumiConcursos.Domain.Entities;
using KurumiConcursos.Infra.ORM.EntitiesMapping.Base;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace KurumiConcursos.Infra.ORM.EntitiesMapping;

public sealed class FocusSessionMapping : MappingBase, IEntityTypeConfiguration<FocusSession>
{
    public void Configure(EntityTypeBuilder<FocusSession> b)
    {
        b.ToTable("focus_session", Schema);
        MappingColumns.Base(b);
        b.Property(x => x.UserId).HasColumnName("user_id");
        b.Property(x => x.JourneyId).HasColumnName("journey_id");
        b.Property(x => x.KnowledgeAreaId).HasColumnName("knowledge_area_id");
        b.Property(x => x.SyllabusNodeId).HasColumnName("syllabus_node_id");
        b.Property(x => x.StudyDate).HasColumnName("study_date");
        b.Property(x => x.DurationSeconds).HasColumnName("duration_seconds");
        b.Property(x => x.Notes).HasColumnName("notes");
    }
}