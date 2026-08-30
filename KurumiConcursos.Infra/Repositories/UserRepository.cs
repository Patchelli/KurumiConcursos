using System.Linq.Expressions;
using KurumiConcursos.Domain.Entities;
using KurumiConcursos.Domain.Enums;
using KurumiConcursos.Domain.Handlers.PaginationHandler;
using KurumiConcursos.Domain.Handlers.PaginationHandler.Filters;
using KurumiConcursos.Infra.Interfaces.RepositoryContracts;
using KurumiConcursos.Infra.Interfaces.ServiceContracts;
using KurumiConcursos.Infra.ORM.Context;
using KurumiConcursos.Infra.Repositories.Base;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Query;

namespace KurumiConcursos.Infra.Repositories;

public sealed class UserRepository(
    ApplicationContext dbContext,
    UserManager<User> userManager,
    IPaginationQueryService<User> paginationQueryService)
    : RepositoryBase<User>(dbContext), IUserRepository
{
    public Task<bool> ExistsAsync(Expression<Func<User, bool>> predicate) =>
        DbSetContext.AsNoTracking().AnyAsync(predicate);

    public Task<PageList<User>> FindAllWithPaginationAsync(UserPageParams pageParams,
        Expression<Func<User, bool>>? predicate = null,
        Func<IQueryable<User>, IIncludableQueryable<User, object>>? include = null)
    {
        IQueryable<User> query = DbSetContext;
        if (predicate is not null) query = query.Where(predicate);
        if (include is not null) query = include(query);
        query = query.OrderByDescending(user => user.CreationDate);
        return paginationQueryService.CreatePaginationAsync(query, pageParams.PageSize, pageParams.PageNumber);
    }

    public Task<User?> FindByPredicateAsync(Expression<Func<User, bool>> predicate,
        Func<IQueryable<User>, IIncludableQueryable<User, object>>? include = null, bool toQuery = false)
    {
        IQueryable<User> query = toQuery ? DbSetContext.AsNoTracking() : DbSetContext;
        if (include is not null) query = include(query);
        return query.FirstOrDefaultAsync(predicate);
    }

    public Task<List<User>> FindAllByPredicateAsync(Expression<Func<User, bool>>? predicate = null,
        Func<IQueryable<User>, IIncludableQueryable<User, object>>? include = null)
    {
        IQueryable<User> query = DbSetContext;
        if (predicate is not null) query = query.Where(predicate);
        if (include is not null) query = include(query);
        return query.AsNoTracking().ToListAsync();
    }

    public Task<IdentityResult> SaveAsync(User user) => userManager.CreateAsync(user);
    public Task<IdentityResult> UpdateAsync(User user) => userManager.UpdateAsync(user);
    public Task<IdentityResult> DeleteAsync(User user) => userManager.DeleteAsync(user);
    public string HashPassword(User user, string password) => userManager.PasswordHasher.HashPassword(user, password);

    public async Task<IdentityResult> PasswordRecoveryAsync(User user, string newPassword)
    {
        var token = await userManager.GeneratePasswordResetTokenAsync(user);
        return await userManager.ResetPasswordAsync(user, token, newPassword);
    }

    public Task<IdentityResult> ChangePasswordAsync(User user, string currentPassword, string newPassword)
    {
        DetachedObject(user);
        return userManager.ChangePasswordAsync(user, currentPassword, newPassword);
    }

    public async Task<bool> SetStatusAsync(IReadOnlyCollection<Guid> userIds, EUserStatus status)
    {
        if (userIds.Count == 0) return true;
        var ids = userIds.Distinct().ToArray();
        var affected = await DbSetContext.Where(user => ids.Contains(user.Id))
            .ExecuteUpdateAsync(setters => setters.SetProperty(user => user.Status, status));
        return affected == ids.Length;
    }

    public async Task<bool> SetStatusAsync(Guid userId, EUserStatus status) =>
        await DbSetContext.Where(user => user.Id == userId)
            .ExecuteUpdateAsync(setters => setters.SetProperty(user => user.Status, status)) == 1;
}