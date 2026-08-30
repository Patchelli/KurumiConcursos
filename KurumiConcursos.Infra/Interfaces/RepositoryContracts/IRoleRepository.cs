using System.Linq.Expressions;
using KurumiConcursos.Domain.Entities.IdentityEntities;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore.Query;

namespace KurumiConcursos.Infra.Interfaces.RepositoryContracts;

public interface IRoleRepository : IDisposable
{
    Task<IdentityResult> SaveAsync(Role role);
    Task<bool> ActivateOrDeactivateAsync(Guid roleId, bool activeOrInactive);
    Task<bool> ExistsInTheDatabaseAsync(Expression<Func<Role, bool>> predicate);

    Task<Role?> FindByPredicateAsync(
        Expression<Func<Role, bool>> predicate,
        Expression<Func<Role, Role>>? projection = null,
        bool toQuery = false);

    Task<List<Role>> FindAllByPredicateAsync(
        Expression<Func<Role, bool>> predicate,
        Expression<Func<Role, Role>>? projection = null);

    Task<List<Role>> FindAllByPredicateByAsync(
        Expression<Func<Role, bool>> predicate,
        Func<IQueryable<Role>, IIncludableQueryable<Role, object>>? include = null,
        bool toQuery = false);
}