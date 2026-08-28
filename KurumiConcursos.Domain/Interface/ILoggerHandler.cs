using KurumiConcursos.Domain.Entities;

namespace KurumiConcursos.Domain.Interface;

public interface ILoggerHandler
{
    void CreateLogger(DomainLogger logger);
    void CreateLogger(DomainLogger logger, object entity);
    bool HasLogger();
    Task SaveInDataBase();
}
