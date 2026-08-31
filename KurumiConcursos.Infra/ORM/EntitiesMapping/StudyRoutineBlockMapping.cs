using KurumiConcursos.Domain.Entities;
using KurumiConcursos.Infra.ORM.EntitiesMapping.Base;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace KurumiConcursos.Infra.ORM.EntitiesMapping;

public sealed class StudyRoutineBlockMapping : MappingBase, IEntityTypeConfiguration<StudyRoutineBlock>
{
    public void Configure(EntityTypeBuilder<StudyRoutineBlock> builder)
    {
        builder.ToTable("study_routine_block", Schema);
        builder.HasKey(x => x.Id);
        builder.Property(x => x.Id).HasColumnType("bigint").HasColumnName("id").HasColumnOrder(1).ValueGeneratedOnAdd()
            .UseIdentityByDefaultColumn().IsRequired();
        builder.Property(x => x.UserId).HasColumnType("uuid").HasColumnName("user_id").HasColumnOrder(2).IsRequired();
        builder.Property(x => x.JourneyId).HasColumnType("bigint").HasColumnName("journey_id").HasColumnOrder(3)
            .IsRequired();
        builder.Property(x => x.StudyRoutineId).HasColumnType("bigint").HasColumnName("study_routine_id")
            .HasColumnOrder(4).IsRequired();
        builder.Property(x => x.SyllabusNodeId).HasColumnType("bigint").HasColumnName("syllabus_node_id")
            .HasColumnOrder(5).IsRequired();
        builder.Property(x => x.ScheduledFor).HasColumnName("scheduled_for").HasColumnOrder(6).IsRequired();
        builder.Property(x => x.Type).HasColumnName("type").HasColumnOrder(7).IsRequired();
        builder.Property(x => x.Status).HasColumnName("status").HasColumnOrder(8).IsRequired();
        builder.Property(x => x.PlannedMinutes).HasColumnName("planned_minutes").HasColumnOrder(9).IsRequired();
        builder.Property(x => x.CompletedMinutes).HasColumnName("completed_minutes").HasColumnOrder(10).IsRequired();
        builder.Property(x => x.Order).HasColumnName("display_order").HasColumnOrder(11).IsRequired();
        builder.Property(x => x.CompletedAt).HasColumnType("timestamptz").HasColumnName("completed_at")
            .HasColumnOrder(12).IsRequired(false);
        builder.Property(x => x.CreationDate).HasColumnType("timestamptz").HasColumnName("creation_date")
            .HasColumnOrder(13).IsRequired();
        builder.Property(x => x.LastUpdateDate).HasColumnType("timestamptz").HasColumnName("last_update_date")
            .HasColumnOrder(14).IsRequired(false);
        builder.HasIndex(x => new { x.UserId, x.ScheduledFor }).HasDatabaseName("ix_study_routine_block_user_date");
    }
}