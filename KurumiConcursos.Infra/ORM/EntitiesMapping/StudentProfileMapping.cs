using KurumiConcursos.Domain.Entities;
using KurumiConcursos.Infra.ORM.EntitiesMapping.Base;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace KurumiConcursos.Infra.ORM.EntitiesMapping;

public sealed class StudentProfileMapping
    : MappingBase, IEntityTypeConfiguration<StudentProfile>
{
    public void Configure(EntityTypeBuilder<StudentProfile> builder)
    {
        builder.ToTable(nameof(StudentProfile), Schema);
        builder.HasKey(profile => profile.Id);

        builder.Property(profile => profile.Id)
            .HasColumnType("bigint")
            .HasColumnName("id")
            .HasColumnOrder(1)
            .ValueGeneratedOnAdd()
            .UseIdentityByDefaultColumn()
            .IsRequired();

        builder.Property(profile => profile.UserId)
            .HasColumnType("uuid")
            .HasColumnName("user_id")
            .HasColumnOrder(2)
            .IsRequired();

        builder.Property(profile => profile.CreationDate)
            .HasColumnType("timestamp with time zone")
            .HasColumnName("created_at")
            .HasColumnOrder(3)
            .IsRequired();

        builder.Property(profile => profile.LastUpdateDate)
            .HasColumnType("timestamp with time zone")
            .HasColumnName("updated_at")
            .HasColumnOrder(4);

        builder.HasOne(profile => profile.User)
            .WithOne(user => user.StudentProfile)
            .HasForeignKey<StudentProfile>(profile => profile.UserId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasMany(profile => profile.ExamJourneys)
            .WithOne(journey => journey.StudentProfile)
            .HasForeignKey(journey => journey.UserId)
            .HasPrincipalKey(profile => profile.UserId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasIndex(profile => profile.UserId)
            .IsUnique()
            .HasDatabaseName("ix_student_profile_user_id");
    }
}
