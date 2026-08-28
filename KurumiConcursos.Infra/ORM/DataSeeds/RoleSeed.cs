using KurumiConcursos.Domain.Entities.IdentityEntities;
using KurumiConcursos.Domain.Enums;

namespace KurumiConcursos.Infra.ORM.DataSeeds;

public static class RoleSeed
{
    public static readonly Guid AdminRoleId = new("B0EEBF72-EB13-4AE9-9389-03E441E81357");
    public static readonly Guid StudentRoleId = new("C3A1D042-712B-4C56-A33E-19DBA2C2BAAB");

    public static List<Role> CreateRolesSeed() =>
    [
        new()
        {
            Id = AdminRoleId, Name = "Administrator", NormalizedName = "ADMINISTRATOR",
            Active = true, Type = ERoleType.Administrator
        },
        new()
        {
            Id = StudentRoleId, Name = "Student", NormalizedName = "STUDENT",
            Active = true, Type = ERoleType.Student
        }
    ];
}
