using KurumiConcursos.Domain.Enums;
using Microsoft.AspNetCore.Identity;

namespace KurumiConcursos.Domain.Entities.IdentityEntities;

public sealed class Role : IdentityRole<Guid>
{
    public bool Active { get; set; }
    public ERoleType Type { get; init; }
    public List<UserRole>? UserRoles { get; set; }
    public List<RoleClaim>? RoleClaims { get; set; }
}