using Microsoft.AspNetCore.Identity;

namespace KurumiConcursos.Domain.Entities.IdentityEntities;

public sealed class UserToken : IdentityUserToken<Guid>
{
}