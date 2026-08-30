using KurumiConcursos.Domain.Entities;

namespace KurumiConcursos.Infra.Interfaces.RepositoryContracts;

public interface ICalendarEventRepository : IDisposable
{
    Task<List<CalendarEvent>> FindAllAsync(Guid userId, CancellationToken cancellationToken);

    Task<CalendarEvent?> FindByIdAsync(long id, Guid userId, CancellationToken cancellationToken,
        bool tracking = false);

    Task<bool> SaveAsync(CalendarEvent entity);
    Task<bool> UpdateAsync(CalendarEvent entity);
    Task<bool> DeleteAsync(CalendarEvent entity);
}