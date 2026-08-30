using KurumiConcursos.Domain.Entities;
using KurumiConcursos.Infra.ORM.EntitiesMapping.Base;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace KurumiConcursos.Infra.ORM.EntitiesMapping;

public sealed class ReviewAppointmentMapping : MappingBase, IEntityTypeConfiguration<ReviewAppointment>
{
    public void Configure(EntityTypeBuilder<ReviewAppointment> b)
    {
        b.ToTable("review_appointment", Schema);
        MappingColumns.Base(b);
        b.Property(x => x.UserId).HasColumnName("user_id");
        b.Property(x => x.SyllabusNodeId).HasColumnName("syllabus_node_id");
        b.Property(x => x.ScheduledFor).HasColumnName("scheduled_for");
        b.Property(x => x.Completed).HasColumnName("completed");
        b.Property(x => x.CompletedAt).HasColumnName("completed_at");
        b.Property(x => x.Superseded).HasColumnName("superseded");
    }
}