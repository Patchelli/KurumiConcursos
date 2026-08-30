using System.Linq.Expressions;
using KurumiConcursos.Domain.Handlers.PaginationHandler;
using Microsoft.EntityFrameworkCore.Query;

namespace KurumiConcursos.Infra.Interfaces.RepositoryContracts.Base;

public interface IReadOnlyRepository<T> where T : class
{
    IQueryable<T> AsQueryable(bool asNoTracking = true);

    Task<TResult?> FindProjectedByPredicateAsync<TResult>(Expression<Func<T, bool>> predicate,
        Expression<Func<T, TResult>> selector, Func<IQueryable<T>, IIncludableQueryable<T, object>>? include = null,
        bool asNoTracking = true, bool splitQuery = false);

    Task<IList<TResult>> FindAllProjectedAsync<TResult>(Expression<Func<T, TResult>> selector,
        Expression<Func<T, bool>>? predicate = null, Func<IQueryable<T>, IOrderedQueryable<T>>? orderBy = null,
        int? take = null, Func<IQueryable<T>, IIncludableQueryable<T, object>>? include = null,
        bool asNoTracking = true);

    Task<PageList<TResult>> FindAllProjectedWithPaginationAsync<TResult>(
        int pageSize,
        int pageNumber,
        Expression<Func<T, TResult>> selector,
        Expression<Func<T, bool>>? predicate = null,
        Func<IQueryable<T>, IOrderedQueryable<T>>? orderBy = null,
        Func<IQueryable<T>, IIncludableQueryable<T, object>>? include = null,
        bool asNoTracking = true) where TResult : class;
}