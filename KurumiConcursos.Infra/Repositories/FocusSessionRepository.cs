using System.Linq.Expressions;
using KurumiConcursos.Domain.Entities;
using KurumiConcursos.Domain.Handlers.PaginationHandler;
using KurumiConcursos.Domain.Handlers.PaginationHandler.Filters;
using KurumiConcursos.Infra.Interfaces.RepositoryContracts;
using KurumiConcursos.Infra.Interfaces.ServiceContracts;
using KurumiConcursos.Infra.ORM.Context;
using KurumiConcursos.Infra.Repositories.Base;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Query;

namespace KurumiConcursos.Infra.Repositories;

public sealed class FocusSessionRepository(
    ApplicationContext dbContext,
    IPaginationQueryService<FocusSession> paginationQueryService)
    : RepositoryBase<FocusSession>(dbContext), IFocusSessionRepository
{
    public async Task<bool> SaveAsync(FocusSession session)
    {
        await DbSetContext.AddAsync(session);
        return await SaveInDatabaseAsync();
    }

    public Task<bool> UpdateAsync(FocusSession session)
    {
        DetachedObject(session);
        DbSetContext.Update(session);
        return SaveInDatabaseAsync();
    }

    public Task<bool> DeleteAsync(FocusSession session)
    {
        DbSetContext.Remove(session);
        return SaveInDatabaseAsync();
    }

    public Task<bool> ExistsAsync(Expression<Func<FocusSession, bool>> predicate) =>
        DbSetContext.AsNoTracking().AnyAsync(predicate);

    public Task<FocusSession?> FindByPredicateAsync(Expression<Func<FocusSession, bool>> predicate,
        Func<IQueryable<FocusSession>, IIncludableQueryable<FocusSession, object>>? include = null,
        bool asNoTracking = false)
    {
        IQueryable<FocusSession> query = DbSetContext;
        if (asNoTracking) query = query.AsNoTracking();
        if (include is not null) query = include(query);
        return query.FirstOrDefaultAsync(predicate);
    }

    public Task<PageList<FocusSession>> FindAllWithPaginationAsync(PageParams pageParams,
        Expression<Func<FocusSession, bool>>? predicate = null,
        Func<IQueryable<FocusSession>, IIncludableQueryable<FocusSession, object>>? include = null)
    {
        IQueryable<FocusSession> query = DbSetContext;
        if (include is not null) query = include(query);
        if (predicate is not null) query = query.Where(predicate);
        query = query.OrderByDescending(x => x.StudyDate).ThenByDescending(x => x.Id);
        return paginationQueryService.CreatePaginationAsync(query, pageParams.PageSize, pageParams.PageNumber);
    }

    public async Task<IList<FocusSession>> FindAllAsync(Expression<Func<FocusSession, bool>>? predicate = null,
        Func<IQueryable<FocusSession>, IIncludableQueryable<FocusSession, object>>? include = null)
    {
        IQueryable<FocusSession> query = DbSetContext;
        if (include is not null) query = include(query);
        if (predicate is not null) query = query.Where(predicate);
        return await query.AsNoTracking().OrderByDescending(x => x.StudyDate).ThenByDescending(x => x.Id).ToListAsync();
    }
}