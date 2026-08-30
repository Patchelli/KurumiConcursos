using KurumiConcursos.Domain.Entities.IdentityEntities;
using KurumiConcursos.Infra.ORM.EntitiesMapping.Base;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace KurumiConcursos.Infra.ORM.EntitiesMapping;

public sealed class UserLoginMapping : MappingBase, IEntityTypeConfiguration<UserLogin>
{
    public void Configure(EntityTypeBuilder<UserLogin> b)
    {
        b.ToTable(nameof(UserLogin), Schema);
        b.HasKey(x => new { x.LoginProvider, x.ProviderKey });
        b.Property(x => x.UserId).HasColumnName("user_id");
    }
}