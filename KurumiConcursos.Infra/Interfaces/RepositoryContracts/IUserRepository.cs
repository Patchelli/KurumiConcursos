using System.Linq.Expressions;
using KurumiConcursos.Domain.Entities;
using KurumiConcursos.Domain.Enums;
using KurumiConcursos.Domain.Handlers.PaginationHandler;
using KurumiConcursos.Domain.Handlers.PaginationHandler.Filters;
using KurumiConcursos.Infra.Interfaces.RepositoryContracts.Base;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore.Query;

namespace KurumiConcursos.Infra.Interfaces.RepositoryContracts;

public interface IUserRepository : IReadOnlyRepository<User>, IDisposable
{
    Task<IdentityResult> SaveAsync(User user);
    Task<IdentityResult> UpdateAsync(User user);
    Task<IdentityResult> DeleteAsync(User user);
    Task<IdentityResult> ChangePasswordAsync(User user, string currentPassword, string newPassword);
    Task<IdentityResult> PasswordRecoveryAsync(User user, string newPassword);
    string HashPassword(User user, string password);
    Task<bool> ExistsAsync(Expression<Func<User, bool>> predicate);
    Task<PageList<User>> FindAllWithPaginationAsync(UserPageParams pageParams,
        Expression<Func<User, bool>>? predicate = null,
        Func<IQueryable<User>, IIncludableQueryable<User, object>>? include = null);
    Task<User?> FindByPredicateAsync(Expression<Func<User, bool>> predicate,
        Func<IQueryable<User>, IIncludableQueryable<User, object>>? include = null,
        bool toQuery = false);
    Task<List<User>> FindAllByPredicateAsync(Expression<Func<User, bool>>? predicate = null,
        Func<IQueryable<User>, IIncludableQueryable<User, object>>? include = null);
    Task<bool> SetStatusAsync(IReadOnlyCollection<Guid> userIds, EUserStatus status);
    Task<bool> SetStatusAsync(Guid userId, EUserStatus status);
}
