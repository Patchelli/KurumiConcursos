using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using KurumiConcursos.Domain.Entities;
using KurumiConcursos.Domain.Entities.IdentityEntities;
using KurumiConcursos.Infra.ORM.EntitiesMapping.Base;

namespace KurumiConcursos.Infra.ORM.EntitiesMapping;

public sealed class UserTokenMapping : MappingBase, IEntityTypeConfiguration<UserToken>
{
    public void Configure(EntityTypeBuilder<UserToken> b)
    {
        b.ToTable(nameof(UserToken), Schema);
        b.HasKey(x => new { x.UserId, x.LoginProvider, x.Name });
        b.Property(x => x.UserId).HasColumnName("user_id");
        b.HasOne<User>().WithMany(x => x.UserTokens).HasForeignKey(x => x.UserId);
    }
}