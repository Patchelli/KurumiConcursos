using System.Linq.Expressions;
using KurumiConcursos.Domain.Entities;
using KurumiConcursos.Domain.Handlers.PaginationHandler;
using KurumiConcursos.Domain.Handlers.PaginationHandler.Filters;
using Microsoft.EntityFrameworkCore.Query;

namespace KurumiConcursos.Infra.Interfaces.RepositoryContracts;

public interface ICalendarEventRepository
{
    Task<bool> SaveAsync(CalendarEvent calendarEvent);
    Task<bool> UpdateAsync(CalendarEvent calendarEvent);
    Task<bool> DeleteAsync(CalendarEvent calendarEvent);
    Task<bool> ExistsAsync(Expression<Func<CalendarEvent, bool>> predicate);

    Task<CalendarEvent?> FindByPredicateAsync(Expression<Func<CalendarEvent, bool>> predicate,
        Func<IQueryable<CalendarEvent>, IIncludableQueryable<CalendarEvent, object>>? include = null,
        bool asNoTracking = false);

    Task<PageList<CalendarEvent>> FindAllWithPaginationAsync(PageParams pageParams,
        Expression<Func<CalendarEvent, bool>>? predicate = null,
        Func<IQueryable<CalendarEvent>, IIncludableQueryable<CalendarEvent, object>>? include = null);

    Task<IList<CalendarEvent>> FindAllAsync(Expression<Func<CalendarEvent, bool>>? predicate = null,
        Func<IQueryable<CalendarEvent>, IIncludableQueryable<CalendarEvent, object>>? include = null);
}