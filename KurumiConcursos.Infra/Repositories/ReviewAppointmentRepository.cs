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

public sealed class ReviewAppointmentRepository(
    ApplicationContext dbContext,
    IPaginationQueryService<ReviewAppointment> paginationQueryService)
    : RepositoryBase<ReviewAppointment>(dbContext), IReviewAppointmentRepository
{
    public async Task<bool> SaveAsync(ReviewAppointment appointment)
    {
        await DbSetContext.AddAsync(appointment);
        return await SaveInDatabaseAsync();
    }

    public Task<bool> UpdateAsync(ReviewAppointment appointment)
    {
        DetachedObject(appointment);
        DbSetContext.Update(appointment);
        return SaveInDatabaseAsync();
    }

    public Task<bool> DeleteAsync(ReviewAppointment appointment)
    {
        DbSetContext.Remove(appointment);
        return SaveInDatabaseAsync();
    }

    public Task<bool> ExistsAsync(Expression<Func<ReviewAppointment, bool>> predicate) =>
        DbSetContext.AsNoTracking().AnyAsync(predicate);

    public Task<ReviewAppointment?> FindByPredicateAsync(Expression<Func<ReviewAppointment, bool>> predicate,
        Func<IQueryable<ReviewAppointment>, IIncludableQueryable<ReviewAppointment, object>>? include = null,
        bool asNoTracking = false)
    {
        IQueryable<ReviewAppointment> query = DbSetContext;
        if (asNoTracking) query = query.AsNoTracking();
        if (include is not null) query = include(query);
        return query.FirstOrDefaultAsync(predicate);
    }

    public Task<PageList<ReviewAppointment>> FindAllWithPaginationAsync(PageParams pageParams,
        Expression<Func<ReviewAppointment, bool>>? predicate = null,
        Func<IQueryable<ReviewAppointment>, IIncludableQueryable<ReviewAppointment, object>>? include = null)
    {
        IQueryable<ReviewAppointment> query = DbSetContext;
        if (include is not null) query = include(query);
        if (predicate is not null) query = query.Where(predicate);
        query = query.OrderBy(x => x.ScheduledFor).ThenBy(x => x.Id);
        return paginationQueryService.CreatePaginationAsync(query, pageParams.PageSize, pageParams.PageNumber);
    }

    public async Task<IList<ReviewAppointment>> FindAllAsync(
        Expression<Func<ReviewAppointment, bool>>? predicate = null,
        Func<IQueryable<ReviewAppointment>, IIncludableQueryable<ReviewAppointment, object>>? include = null)
    {
        IQueryable<ReviewAppointment> query = DbSetContext;
        if (include is not null) query = include(query);
        if (predicate is not null) query = query.Where(predicate);
        return await query.AsNoTracking().OrderBy(x => x.ScheduledFor).ThenBy(x => x.Id).ToListAsync();
    }
}