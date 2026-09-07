using KurumiConcursos.Domain.Entities;
using KurumiConcursos.Infra.ORM.EntitiesMapping.Base;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace KurumiConcursos.Infra.ORM.EntitiesMapping;

public sealed class QuestionAppointmentMapping : MappingBase, IEntityTypeConfiguration<QuestionAppointment>
{
    public void Configure(EntityTypeBuilder<QuestionAppointment> builder)
    {
        builder.ToTable("question_appointment", Schema);
        MappingColumns.Base(builder);
        builder.Property(item => item.UserId).HasColumnName("user_id");
        builder.Property(item => item.JourneyId).HasColumnName("journey_id");
        builder.Property(item => item.SyllabusNodeId).HasColumnName("syllabus_node_id");
        builder.Property(item => item.ScheduledFor).HasColumnName("scheduled_for");
        builder.Property(item => item.Completed).HasColumnName("completed");
        builder.Property(item => item.CompletedAt).HasColumnName("completed_at");
        builder.Property(item => item.Superseded).HasColumnName("superseded");
        builder.Property(item => item.AdaptationTrigger).HasColumnName("adaptation_trigger");
        builder.HasOne(item => item.SyllabusNode).WithMany().HasForeignKey(item => item.SyllabusNodeId);
        builder.HasIndex(item => new { item.UserId, item.JourneyId, item.ScheduledFor });
    }
}