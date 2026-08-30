using KurumiConcursos.Domain.Entities;
using KurumiConcursos.Infra.ORM.EntitiesMapping.Base;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace KurumiConcursos.Infra.ORM.EntitiesMapping;

public sealed class PersonalDataMapping : MappingBase, IEntityTypeConfiguration<PersonalData>
{
    public void Configure(EntityTypeBuilder<PersonalData> builder)
    {
        builder.ToTable(nameof(PersonalData), Schema);
        builder.HasKey(p => p.Id);

        builder.Property(p => p.Id)
            .HasColumnType("bigint")
            .HasColumnName("id")
            .HasColumnOrder(1)
            .ValueGeneratedOnAdd()
            .UseIdentityByDefaultColumn()
            .IsRequired();

        builder.Property(p => p.UserId)
            .HasColumnType("uuid")
            .HasColumnName("user_id")
            .HasColumnOrder(2)
            .IsRequired();

        builder.Property(p => p.FullName)
            .HasColumnType("varchar(255)")
            .HasColumnName("full_name")
            .HasColumnOrder(3)
            .IsRequired(false);

        builder.Property(p => p.Document)
            .HasColumnType("varchar(50)")
            .HasColumnName("document")
            .HasColumnOrder(4)
            .IsRequired(false);

        builder.Property(p => p.Phone)
            .HasColumnType("varchar(50)")
            .HasColumnName("phone")
            .HasColumnOrder(5)
            .IsRequired(false);

        builder.Property(p => p.Age)
            .HasColumnType("integer")
            .HasColumnName("age")
            .HasColumnOrder(6)
            .IsRequired(false);

        builder.Property(p => p.DateOfBirth)
            .HasColumnType("timestamptz")
            .HasColumnName("date_of_birth")
            .HasColumnOrder(7)
            .IsRequired(false);

        builder.Property(p => p.CreationDate)
            .HasColumnType("timestamptz")
            .HasColumnName("created_at")
            .HasColumnOrder(8)
            .IsRequired();

        builder.Property(p => p.LastUpdateDate)
            .HasColumnType("timestamptz")
            .HasColumnName("updated_at")
            .HasColumnOrder(9)
            .IsRequired(false);

        builder.HasIndex(p => p.UserId)
            .IsUnique()
            .HasDatabaseName("ux_personal_data_user_id");

        builder.HasOne(p => p.User)
            .WithOne(u => u.PersonalData)
            .HasForeignKey<PersonalData>(p => p.UserId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}