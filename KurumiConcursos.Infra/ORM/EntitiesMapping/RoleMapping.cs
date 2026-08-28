using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using KurumiConcursos.Domain.Entities.IdentityEntities;
using KurumiConcursos.Infra.ORM.EntitiesMapping.Base;

namespace KurumiConcursos.Infra.ORM.EntitiesMapping;

public sealed class RoleMapping : MappingBase, IEntityTypeConfiguration<Role>
{
    public void Configure(EntityTypeBuilder<Role> b)
    {
        b.ToTable(nameof(Role), Schema);
        b.Property(x => x.Id).HasColumnName("id");
        b.Property(x => x.Name).HasColumnName("name");
        b.Property(x => x.NormalizedName).HasColumnName("normalized_name");
        b.Property(x => x.Active).HasColumnName("active");
        b.Property(x => x.Type).HasColumnName("type");
        b.HasMany(x => x.UserRoles).WithOne(x => x.Role).HasForeignKey(x => x.RoleId);
        b.HasMany(x => x.RoleClaims).WithOne().HasForeignKey(x => x.RoleId);
    }
}