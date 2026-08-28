using Microsoft.AspNetCore.Identity;

namespace KurumiConcursos.Domain.Entities.IdentityEntities;

public sealed class UserRole : IdentityUserRole<Guid>
{
    public Role? Role { get; set; }
    public User? User { get; set; }
}