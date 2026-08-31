using System.Linq.Expressions;
using KurumiConcursos.Domain.Entities;
using KurumiConcursos.Domain.Handlers.PaginationHandler;
using KurumiConcursos.Domain.Handlers.PaginationHandler.Filters;
using Microsoft.EntityFrameworkCore.Query;

namespace KurumiConcursos.Infra.Interfaces.RepositoryContracts;

public interface IFocusSessionRepository
{
    Task<bool> SaveAsync(FocusSession session);
    Task<bool> UpdateAsync(FocusSession session);
    Task<bool> DeleteAsync(FocusSession session);
    Task<bool> ExistsAsync(Expression<Func<FocusSession, bool>> predicate);

    Task<FocusSession?> FindByPredicateAsync(Expression<Func<FocusSession, bool>> predicate,
        Func<IQueryable<FocusSession>, IIncludableQueryable<FocusSession, object>>? include = null,
        bool asNoTracking = false);

    Task<PageList<FocusSession>> FindAllWithPaginationAsync(PageParams pageParams,
        Expression<Func<FocusSession, bool>>? predicate = null,
        Func<IQueryable<FocusSession>, IIncludableQueryable<FocusSession, object>>? include = null);

    Task<IList<FocusSession>> FindAllAsync(Expression<Func<FocusSession, bool>>? predicate = null,
        Func<IQueryable<FocusSession>, IIncludableQueryable<FocusSession, object>>? include = null);
}