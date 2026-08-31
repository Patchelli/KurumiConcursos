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

public sealed class CalendarEventRepository(
    ApplicationContext dbContext,
    IPaginationQueryService<CalendarEvent> paginationQueryService)
    : RepositoryBase<CalendarEvent>(dbContext), ICalendarEventRepository
{
    public async Task<bool> SaveAsync(CalendarEvent calendarEvent)
    {
        await DbSetContext.AddAsync(calendarEvent);
        return await SaveInDatabaseAsync();
    }

    public Task<bool> UpdateAsync(CalendarEvent calendarEvent)
    {
        DetachedObject(calendarEvent);
        DbSetContext.Update(calendarEvent);
        return SaveInDatabaseAsync();
    }

    public Task<bool> DeleteAsync(CalendarEvent calendarEvent)
    {
        DbSetContext.Remove(calendarEvent);
        return SaveInDatabaseAsync();
    }

    public Task<bool> ExistsAsync(Expression<Func<CalendarEvent, bool>> predicate) =>
        DbSetContext.AsNoTracking().AnyAsync(predicate);

    public Task<CalendarEvent?> FindByPredicateAsync(Expression<Func<CalendarEvent, bool>> predicate,
        Func<IQueryable<CalendarEvent>, IIncludableQueryable<CalendarEvent, object>>? include = null,
        bool asNoTracking = false)
    {
        IQueryable<CalendarEvent> query = DbSetContext;
        if (asNoTracking) query = query.AsNoTracking();
        if (include is not null) query = include(query);
        return query.FirstOrDefaultAsync(predicate);
    }

    public Task<PageList<CalendarEvent>> FindAllWithPaginationAsync(PageParams pageParams,
        Expression<Func<CalendarEvent, bool>>? predicate = null,
        Func<IQueryable<CalendarEvent>, IIncludableQueryable<CalendarEvent, object>>? include = null)
    {
        IQueryable<CalendarEvent> query = DbSetContext;
        if (include is not null) query = include(query);
        if (predicate is not null) query = query.Where(predicate);
        query = query.OrderBy(x => x.Date).ThenBy(x => x.Id);
        return paginationQueryService.CreatePaginationAsync(query, pageParams.PageSize, pageParams.PageNumber);
    }

    public async Task<IList<CalendarEvent>> FindAllAsync(Expression<Func<CalendarEvent, bool>>? predicate = null,
        Func<IQueryable<CalendarEvent>, IIncludableQueryable<CalendarEvent, object>>? include = null)
    {
        IQueryable<CalendarEvent> query = DbSetContext;
        if (include is not null) query = include(query);
        if (predicate is not null) query = query.Where(predicate);
        return await query.AsNoTracking().OrderBy(x => x.Date).ThenBy(x => x.Id).ToListAsync();
    }
}