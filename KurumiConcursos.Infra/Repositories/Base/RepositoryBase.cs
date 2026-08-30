using System.Linq.Expressions;
using KurumiConcursos.Domain.Handlers.PaginationHandler;
using KurumiConcursos.Infra.Interfaces.RepositoryContracts.Base;
using KurumiConcursos.Infra.ORM.Context;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Query;

namespace KurumiConcursos.Infra.Repositories.Base;

public abstract class RepositoryBase<T>(ApplicationContext dbContext)
    : IDisposable, IReadOnlyRepository<T> where T : class
{
    private const int StandardQuantity = 0;

    protected DbSet<T> DbSetContext => dbContext.Set<T>();
    protected ApplicationContext Context => dbContext;

    public IQueryable<T> AsQueryable(bool asNoTracking = true) =>
        asNoTracking ? DbSetContext.AsNoTracking() : DbSetContext;

    protected IQueryable<T> Query(bool asNoTracking) =>
        asNoTracking ? DbSetContext.AsNoTracking() : DbSetContext;

    protected async Task<bool> SaveInDatabaseAsync(CancellationToken cancellationToken = default) =>
        await dbContext.SaveChangesAsync(cancellationToken) > StandardQuantity;

    protected void DetachedObject(T entity)
    {
        if (dbContext.Entry(entity).State == EntityState.Detached)
            DbSetContext.Attach(entity);
    }

    protected void AddEntity(T entity) => DbSetContext.Add(entity);
    protected void AddRangeEntities(IReadOnlyCollection<T> entities) => DbSetContext.AddRange(entities);

    protected void UpdateEntity(T entity)
    {
        DetachedObject(entity);
        DbSetContext.Update(entity);
    }

    protected void UpdateRangeEntities(IReadOnlyCollection<T> entities) => DbSetContext.UpdateRange(entities);
    protected void RemoveEntity(T entity) => DbSetContext.Remove(entity);
    protected void RemoveRangeEntities(IReadOnlyCollection<T> entities) => DbSetContext.RemoveRange(entities);

    public Task<TResult?> FindProjectedByPredicateAsync<TResult>(
        Expression<Func<T, bool>> predicate,
        Expression<Func<T, TResult>> selector,
        Func<IQueryable<T>, IIncludableQueryable<T, object>>? include = null,
        bool asNoTracking = true,
        bool splitQuery = false)
    {
        IQueryable<T> query = Query(asNoTracking);
        if (include is not null) query = include(query);
        if (splitQuery) query = query.AsSplitQuery();
        return query.Where(predicate).Select(selector).FirstOrDefaultAsync();
    }

    public async Task<IList<TResult>> FindAllProjectedAsync<TResult>(
        Expression<Func<T, TResult>> selector,
        Expression<Func<T, bool>>? predicate = null,
        Func<IQueryable<T>, IOrderedQueryable<T>>? orderBy = null,
        int? take = null,
        Func<IQueryable<T>, IIncludableQueryable<T, object>>? include = null,
        bool asNoTracking = true)
    {
        IQueryable<T> query = Query(asNoTracking);
        if (include is not null) query = include(query);
        if (predicate is not null) query = query.Where(predicate);
        if (orderBy is not null) query = orderBy(query);
        if (take is not null) query = query.Take(take.Value);
        return await query.Select(selector).ToListAsync();
    }

    public async Task<PageList<TResult>> FindAllProjectedWithPaginationAsync<TResult>(
        int pageSize,
        int pageNumber,
        Expression<Func<T, TResult>> selector,
        Expression<Func<T, bool>>? predicate = null,
        Func<IQueryable<T>, IOrderedQueryable<T>>? orderBy = null,
        Func<IQueryable<T>, IIncludableQueryable<T, object>>? include = null,
        bool asNoTracking = true) where TResult : class
    {
        IQueryable<T> query = Query(asNoTracking);
        if (include is not null) query = include(query);
        if (predicate is not null) query = query.Where(predicate);
        var totalCount = await query.CountAsync();
        if (orderBy is not null) query = orderBy(query);
        var items = await query.Skip((pageNumber - 1) * pageSize).Take(pageSize).Select(selector).ToListAsync();
        return new PageList<TResult>(items, totalCount, pageNumber, pageSize);
    }

    public void Dispose()
    {
        dbContext.Dispose();
        GC.SuppressFinalize(this);
    }
}