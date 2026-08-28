using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using KurumiConcursos.Domain.Entities;
using KurumiConcursos.Infra.ORM.EntitiesMapping.Base;

namespace KurumiConcursos.Infra.ORM.EntitiesMapping;

public sealed class StudyRoutineMapping : MappingBase, IEntityTypeConfiguration<StudyRoutine>
{
    public void Configure(EntityTypeBuilder<StudyRoutine> b)
    {
        b.ToTable("study_routine", Schema);
        MappingColumns.Base(b);
        b.Property(x => x.UserId).HasColumnName("user_id");
        b.Property(x => x.JourneyId).HasColumnName("journey_id");
        b.Property(x => x.Title).HasColumnName("title");
        b.Property(x => x.Kind).HasColumnName("kind");
        b.Property(x => x.Active).HasColumnName("active");
        b.Property(x => x.ConfigurationJson).HasColumnName("configuration_json").HasColumnType("jsonb");
    }
}