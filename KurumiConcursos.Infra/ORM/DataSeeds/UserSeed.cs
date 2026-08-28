using KurumiConcursos.Domain.Entities;
using KurumiConcursos.Domain.Entities.IdentityEntities;
using KurumiConcursos.Domain.Enums;

namespace KurumiConcursos.Infra.ORM.DataSeeds;

public static class UserSeed
{
    public static readonly Guid AdminUserId = Guid.NewGuid();
    public static readonly Guid AdminRoleId = new("B0EEBF72-EB13-4AE9-9389-03E441E81357");

    private static readonly DateTimeOffset AdminBaseDate =
        new(2025, 01, 01, 0, 0, 0, TimeSpan.Zero);

    public static readonly Dictionary<Guid, string> DefaultPasswords = new()
    {
        { AdminUserId, "manager@kurumiconcursos2026" }
    };

    public static List<User> CreateUserSeed() =>
    [
        new()
        {
            Id = AdminUserId,
            UserName = "admin@kurumiconcursos.com",
            NormalizedUserName = "ADMIN@KURUMICONCURSOS.COM",
            Email = "admin@kurumiconcursos.com",
            NormalizedEmail = "ADMIN@KURUMICONCURSOS.COM",
            EmailConfirmed = true,
            Status = EUserStatus.Active,
            CreationDate = AdminBaseDate,
            LastAccessDate = AdminBaseDate,
            Identifier = "ADMIN-SYSTEM",
            PreferredLanguage = ELanguage.PtBr,
            UserRoles = [new UserRole { UserId = AdminUserId, RoleId = AdminRoleId }]
        }
    ];

    public static List<PersonalData> CreatePersonalDataSeed() =>
    [
        new PersonalData
        {
            UserId = AdminUserId,
            FullName = "Administrador Sistema",
            Document = null
        }
    ];

    public static List<AdminProfile> CreateAdminProfilesSeed() =>
    [
        new AdminProfile
        {
            UserId = AdminUserId
        }
    ];
}
