using KurumiConcursos.Domain.Entities;
using KurumiConcursos.Domain.Interface;
using KurumiConcursos.Infra.ORM.Context;

namespace KurumiConcursos.ApplicationService.Services.LoggerHandlerServices;

public sealed class LoggerHandler(ApplicationContext context) : ILoggerHandler
{
    private readonly List<DomainLogger> _buffer = [];
    private readonly List<(DomainLogger Log, object Entity)> _pending = [];

    public void CreateLogger(DomainLogger logger) => _buffer.Add(logger);

    public void CreateLogger(DomainLogger logger, object entity)
    {
        _buffer.Add(logger);
        _pending.Add((logger, entity));
    }

    public bool HasLogger() => _buffer.Count > 0;

    public async Task SaveInDataBase()
    {
        foreach (var (log, entity) in _pending)
        {
            if (!string.IsNullOrWhiteSpace(log.EntityId))
                continue;

            var id = TryGetId(entity);
            if (id is not null)
                log.EntityId = id;
        }

        context.Set<DomainLogger>().AddRange(_buffer);
        await context.SaveChangesAsync();

        _buffer.Clear();
        _pending.Clear();
    }

    private static string? TryGetId(object entity)
    {
        var property = entity.GetType().GetProperty("Id");
        if (property is null) return null;

        var value = property.GetValue(entity);
        if (value is null) return null;

        return value switch
        {
            long number when number > 0 => number.ToString(),
            int number when number > 0 => number.ToString(),
            Guid identifier when identifier != Guid.Empty => identifier.ToString(),
            _ => null
        };
    }
}
