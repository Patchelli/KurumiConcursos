using KurumiConcursos.Domain.Entities;
using KurumiConcursos.Infra.ORM.EntitiesMapping.Base;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace KurumiConcursos.Infra.ORM.EntitiesMapping;

public sealed class AdminProfileMapping : MappingBase, IEntityTypeConfiguration<AdminProfile>
{
    public void Configure(EntityTypeBuilder<AdminProfile> builder)
    {
        builder.ToTable(nameof(AdminProfile), Schema);
        builder.HasKey(a => a.Id);

        builder.Property(a => a.Id)
            .HasColumnType("bigint")
            .HasColumnName("id")
            .HasColumnOrder(1)
            .ValueGeneratedOnAdd()
            .UseIdentityByDefaultColumn()
            .IsRequired();

        builder.Property(a => a.UserId)
            .HasColumnType("uuid")
            .HasColumnName("user_id")
            .HasColumnOrder(2)
            .IsRequired();

        builder.HasOne(a => a.User)
            .WithOne(u => u.AdminProfile)
            .HasForeignKey<AdminProfile>(a => a.UserId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasIndex(a => a.UserId)
            .IsUnique()
            .HasDatabaseName("ix_admin_profile_user_id");
    }
}