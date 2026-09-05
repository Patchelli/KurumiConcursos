using KurumiConcursos.Domain.Entities;
using KurumiConcursos.Infra.ORM.EntitiesMapping.Base;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace KurumiConcursos.Infra.ORM.EntitiesMapping;

public sealed class StudySummaryMapping : MappingBase, IEntityTypeConfiguration<StudySummary>
{
    public void Configure(EntityTypeBuilder<StudySummary> b)
    {
        b.ToTable("study_summary", Schema);
        MappingColumns.Base(b);
        b.Property(x => x.UserId).HasColumnName("user_id");
        b.Property(x => x.JourneyId).HasColumnName("journey_id");
        b.Property(x => x.SyllabusNodeId).HasColumnName("syllabus_node_id");
        b.Property(x => x.ReviewAppointmentId).HasColumnName("review_appointment_id");
        b.Property(x => x.IsReview).HasColumnName("is_review");
        b.Property(x => x.Content).HasColumnName("content").HasMaxLength(10000);
        b.HasIndex(x => new { x.UserId, x.SyllabusNodeId, x.CreationDate });
    }
}