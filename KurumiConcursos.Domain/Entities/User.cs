using Microsoft.AspNetCore.Identity;
using KurumiConcursos.Domain.Entities.IdentityEntities;
using KurumiConcursos.Domain.Enums;

namespace KurumiConcursos.Domain.Entities;

public class User : IdentityUser<Guid>
{
    public EUserStatus Status { get; set; } = EUserStatus.Active;
    public string Identifier { get; set; } = Guid.NewGuid().ToString("N");
    public ELanguage PreferredLanguage { get; set; } = ELanguage.PtBr;
    public DateTimeOffset CreationDate { get; set; } = DateTimeOffset.UtcNow;
    public DateTimeOffset? LastAccessDate { get; set; }
    public List<UserRole>? UserRoles { get; set; }
    public List<UserClaim>? UserClaims { get; set; }
    public List<UserToken>? UserTokens { get; set; }
    public PersonalData? PersonalData { get; set; }
    public AdminProfile? AdminProfile { get; set; }
    public StudentProfile? StudentProfile { get; set; }
}
