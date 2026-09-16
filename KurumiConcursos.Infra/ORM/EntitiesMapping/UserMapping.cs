using KurumiConcursos.Domain.Entities;
using KurumiConcursos.Infra.ORM.EntitiesMapping.Base;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace KurumiConcursos.Infra.ORM.EntitiesMapping;

public sealed class UserMapping : MappingBase, IEntityTypeConfiguration<User>
{
    public void Configure(EntityTypeBuilder<User> builder)
    {
        builder.ToTable(nameof(User), Schema);
        builder.HasKey(user => user.Id);

        builder.Property(user => user.Id)
            .HasColumnType("uuid")
            .HasColumnName("id")
            .HasColumnOrder(1);

        builder.Property(user => user.UserName)
            .HasColumnType("varchar(256)")
            .HasColumnName("username")
            .HasColumnOrder(2);

        builder.Property(user => user.NormalizedUserName)
            .HasColumnType("varchar(256)")
            .HasColumnName("normalized_username")
            .HasColumnOrder(3);

        builder.Property(user => user.Email)
            .HasColumnType("varchar(256)")
            .HasColumnName("email")
            .HasColumnOrder(4);

        builder.Property(user => user.NormalizedEmail)
            .HasColumnType("varchar(256)")
            .HasColumnName("normalized_email")
            .HasColumnOrder(5);

        builder.Property(user => user.EmailConfirmed)
            .HasColumnType("boolean")
            .HasColumnName("email_confirmed")
            .HasColumnOrder(6);

        builder.Property(user => user.PasswordHash)
            .HasColumnType("text")
            .HasColumnName("password_hash")
            .HasColumnOrder(7);

        builder.Property(user => user.SecurityStamp)
            .HasColumnType("text")
            .HasColumnName("security_stamp")
            .HasColumnOrder(8);

        builder.Property(user => user.ConcurrencyStamp)
            .HasColumnType("text")
            .HasColumnName("concurrency_stamp")
            .HasColumnOrder(9);

        builder.Property(user => user.PhoneNumber)
            .HasColumnType("varchar(50)")
            .HasColumnName("phone_number")
            .HasColumnOrder(10);

        builder.Property(user => user.PhoneNumberConfirmed)
            .HasColumnType("boolean")
            .HasColumnName("phone_number_confirmed")
            .HasColumnOrder(11);

        builder.Property(user => user.TwoFactorEnabled)
            .HasColumnType("boolean")
            .HasColumnName("two_factor_enabled")
            .HasColumnOrder(12);

        builder.Property(user => user.LockoutEnd)
            .HasColumnType("timestamp with time zone")
            .HasColumnName("lockout_end")
            .HasColumnOrder(13);

        builder.Property(user => user.LockoutEnabled)
            .HasColumnType("boolean")
            .HasColumnName("lockout_enabled")
            .HasColumnOrder(14);

        builder.Property(user => user.AccessFailedCount)
            .HasColumnType("integer")
            .HasColumnName("access_failed_count")
            .HasColumnOrder(15);

        builder.Property(user => user.Identifier)
            .HasColumnType("varchar(100)")
            .HasColumnName("identifier")
            .HasColumnOrder(16);

        builder.Property(user => user.Status)
            .HasColumnType("integer")
            .HasColumnName("status")
            .HasColumnOrder(17)
            .IsRequired();

        builder.Property(user => user.PreferredLanguage)
            .HasColumnType("integer")
            .HasColumnName("preferred_language")
            .HasColumnOrder(18)
            .IsRequired();

        builder.Property(user => user.CreationDate)
            .HasColumnType("timestamp with time zone")
            .HasColumnName("creation_date")
            .HasColumnOrder(19)
            .IsRequired();

        builder.Property(user => user.LastAccessDate)
            .HasColumnType("timestamp with time zone")
            .HasColumnName("last_access_date")
            .HasColumnOrder(20);

        builder.HasOne(user => user.PersonalData)
            .WithOne(personalData => personalData.User)
            .HasForeignKey<PersonalData>(personalData => personalData.UserId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasOne(user => user.AdminProfile)
            .WithOne(adminProfile => adminProfile.User)
            .HasForeignKey<AdminProfile>(adminProfile => adminProfile.UserId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasOne(user => user.StudentProfile)
            .WithOne(studentProfile => studentProfile.User)
            .HasForeignKey<StudentProfile>(studentProfile => studentProfile.UserId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasMany(user => user.UserRoles)
            .WithOne()
            .HasForeignKey(userRole => userRole.UserId);

        builder.HasMany(user => user.UserClaims)
            .WithOne()
            .HasForeignKey(userClaim => userClaim.UserId);

        builder.HasMany(user => user.UserTokens)
            .WithOne()
            .HasForeignKey(userToken => userToken.UserId);

        builder.HasIndex(user => user.Identifier)
            .IsUnique()
            .HasDatabaseName("ux_user_identifier");
    }
}