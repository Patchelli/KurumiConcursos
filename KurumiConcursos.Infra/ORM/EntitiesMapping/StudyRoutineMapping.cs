using KurumiConcursos.Domain.Entities;
using KurumiConcursos.Infra.ORM.EntitiesMapping.Base;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace KurumiConcursos.Infra.ORM.EntitiesMapping;

public sealed class StudyRoutineMapping : MappingBase, IEntityTypeConfiguration<StudyRoutine>
{
    public void Configure(EntityTypeBuilder<StudyRoutine> builder)
    {
        builder.ToTable("study_routine", Schema);
        builder.HasKey(x => x.Id);
        builder.Property(x => x.Id).HasColumnType("bigint").HasColumnName("id").HasColumnOrder(1).ValueGeneratedOnAdd()
            .UseIdentityByDefaultColumn().IsRequired();
        builder.Property(x => x.UserId).HasColumnType("uuid").HasColumnName("user_id").HasColumnOrder(2).IsRequired();
        builder.Property(x => x.JourneyId).HasColumnType("bigint").HasColumnName("journey_id").HasColumnOrder(3)
            .IsRequired();
        builder.Property(x => x.Title).HasColumnType("varchar(180)").HasColumnName("title").HasColumnOrder(4)
            .IsRequired();
        builder.Property(x => x.Kind).HasColumnName("kind").HasColumnOrder(5).IsRequired();
        builder.Property(x => x.Active).HasColumnName("active").HasColumnOrder(6).IsRequired();
        builder.Property(x => x.ConfigurationJson).HasColumnName("configuration_json").HasColumnType("jsonb")
            .HasColumnOrder(7).IsRequired();
        builder.Property(x => x.CreationDate).HasColumnType("timestamptz").HasColumnName("creation_date")
            .HasColumnOrder(8).IsRequired();
        builder.Property(x => x.LastUpdateDate).HasColumnType("timestamptz").HasColumnName("last_update_date")
            .HasColumnOrder(9).IsRequired(false);
        builder.HasIndex(x => new { x.UserId, x.JourneyId }).HasDatabaseName("ix_study_routine_user_journey");
    }
}