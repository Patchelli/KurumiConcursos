using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using KurumiConcursos.Domain.Entities;
using KurumiConcursos.Infra.ORM.EntitiesMapping.Base;

namespace KurumiConcursos.Infra.ORM.EntitiesMapping;

public sealed class UserMapping : MappingBase, IEntityTypeConfiguration<User>
{
    public void Configure(EntityTypeBuilder<User> b)
    {
        b.ToTable(nameof(User), Schema);
        b.HasKey(x => x.Id);
        b.Property(x => x.Id).HasColumnName("id").HasColumnType("uuid");
        b.Property(x => x.Name).HasColumnName("name").HasMaxLength(120);
        b.Property(x => x.UserName).HasColumnName("username").HasMaxLength(256);
        b.Property(x => x.NormalizedUserName).HasColumnName("normalized_username").HasMaxLength(256);
        b.Property(x => x.Email).HasColumnName("email").HasMaxLength(256);
        b.Property(x => x.NormalizedEmail).HasColumnName("normalized_email").HasMaxLength(256);
        b.Property(x => x.PasswordHash).HasColumnName("password_hash");
        b.Property(x => x.Status).HasColumnName("status");
        b.Property(x => x.Identifier).HasColumnName("identifier").HasMaxLength(100);
        b.Property(x => x.PreferredLanguage).HasColumnName("preferred_language");
        b.Property(x => x.CreationDate).HasColumnName("creation_date");
        b.Property(x => x.LastAccessDate).HasColumnName("last_access_date");
        b.HasIndex(x => x.Identifier).IsUnique().HasDatabaseName("ux_user_identifier");
    }
}