using KurumiConcursos.Domain.Entities.IdentityEntities;
using KurumiConcursos.Infra.ORM.EntitiesMapping.Base;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace KurumiConcursos.Infra.ORM.EntitiesMapping;

public sealed class RoleClaimMapping : MappingBase, IEntityTypeConfiguration<RoleClaim>
{
    public void Configure(EntityTypeBuilder<RoleClaim> b)
    {
        b.ToTable(nameof(RoleClaim), Schema);
        b.Property(x => x.Id).HasColumnName("id");
        b.Property(x => x.RoleId).HasColumnName("role_id");
        b.HasOne<Role>().WithMany(x => x.RoleClaims).HasForeignKey(x => x.RoleId);
    }
}