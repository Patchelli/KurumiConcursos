using KurumiConcursos.Domain.Entities;
using KurumiConcursos.Infra.Interfaces.RepositoryContracts;
using KurumiConcursos.Infra.ORM.Context;
using Microsoft.EntityFrameworkCore;

namespace KurumiConcursos.Infra.Repositories;

public sealed class CalendarEventRepository(ApplicationContext context) : ICalendarEventRepository
{
    public Task<List<CalendarEvent>> FindAllAsync(Guid userId, CancellationToken ct) =>
        context.Set<CalendarEvent>().AsNoTracking().Where(x => x.UserId == userId)
            .OrderBy(x => x.Date).ThenBy(x => x.Id).ToListAsync(ct);

    public Task<CalendarEvent?> FindByIdAsync(long id, Guid userId, CancellationToken ct, bool tracking = false) =>
        (tracking ? context.Set<CalendarEvent>() : context.Set<CalendarEvent>().AsNoTracking())
        .FirstOrDefaultAsync(x => x.Id == id && x.UserId == userId, ct);

    public async Task<bool> SaveAsync(CalendarEvent entity)
    {
        context.Set<CalendarEvent>().Add(entity);
        return await context.SaveChangesAsync() > 0;
    }

    public async Task<bool> UpdateAsync(CalendarEvent entity)
    {
        context.Set<CalendarEvent>().Update(entity);
        return await context.SaveChangesAsync() > 0;
    }

    public async Task<bool> DeleteAsync(CalendarEvent entity)
    {
        context.Set<CalendarEvent>().Remove(entity);
        return await context.SaveChangesAsync() > 0;
    }

    public void Dispose() => context.Dispose();
}