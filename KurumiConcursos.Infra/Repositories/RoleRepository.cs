using System.Linq.Expressions;
using KurumiConcursos.Domain.Entities.IdentityEntities;
using KurumiConcursos.Infra.Interfaces.RepositoryContracts;
using KurumiConcursos.Infra.ORM.Context;
using KurumiConcursos.Infra.Repositories.Base;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Query;

namespace KurumiConcursos.Infra.Repositories;

public sealed class RoleRepository(ApplicationContext dbContext, RoleManager<Role> roleManager)
    : RepositoryBase<Role>(dbContext), IRoleRepository
{
    private const int NumberChangesInDatabase = 1;

    public Task<bool> ExistsInTheDatabaseAsync(Expression<Func<Role, bool>> predicate) =>
        DbSetContext.AnyAsync(predicate);

    public Task<Role?> FindByPredicateAsync(
        Expression<Func<Role, bool>> predicate,
        Expression<Func<Role, Role>>? projection = null,
        bool toQuery = false)
    {
        IQueryable<Role> query = DbSetContext;
        if (toQuery) query = query.AsNoTracking();
        if (projection is not null) query = query.Select(projection);
        return query.FirstOrDefaultAsync(predicate);
    }

    public Task<List<Role>> FindAllByPredicateAsync(
        Expression<Func<Role, bool>> predicate,
        Expression<Func<Role, Role>>? projection = null)
    {
        IQueryable<Role> query = DbSetContext;
        if (projection is not null) query = query.Select(projection);
        return query.Where(predicate).AsNoTracking().ToListAsync();
    }

    public Task<List<Role>> FindAllByPredicateByAsync(
        Expression<Func<Role, bool>> predicate,
        Func<IQueryable<Role>, IIncludableQueryable<Role, object>>? include = null,
        bool toQuery = false)
    {
        IQueryable<Role> query = DbSetContext;
        if (include is not null) query = include(query);
        query = query.Where(predicate);
        return query.AsNoTracking().ToListAsync();
    }

    public Task<IdentityResult> SaveAsync(Role role) => roleManager.CreateAsync(role);

    public async Task<bool> ActivateOrDeactivateAsync(Guid roleId, bool activeOrInactive) =>
        await DbSetContext.Where(role => role.Id == roleId)
            .ExecuteUpdateAsync(setter => setter.SetProperty(role => role.Active, activeOrInactive)) ==
        NumberChangesInDatabase;
}
