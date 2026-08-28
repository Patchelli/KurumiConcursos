using KurumiConcursos.Domain.Interface;
using KurumiConcursos.Infra.ORM.Context;
using Microsoft.EntityFrameworkCore.Infrastructure;

namespace KurumiConcursos.Infra.ORM.UoW;

public sealed class UnitOfWork(
    ApplicationContext applicationContext,
    ILoggerHandler loggerHandler)
    : IUnitOfWork
{
    private readonly DatabaseFacade _databaseFacade = applicationContext.Database;
    private readonly List<Func<Task>> _postCommitActions = [];

    public void BeginTransaction() => _databaseFacade.BeginTransaction();

    public void RegisterPostCommitAction(Func<Task> action) => _postCommitActions.Add(action);

    public async Task CommitAsync(CancellationToken cancellationToken = default)
    {
        try
        {
            await applicationContext.SaveChangesAsync(cancellationToken);
            _databaseFacade.CommitTransaction();

            if (loggerHandler.HasLogger())
                await loggerHandler.SaveInDataBase();
        }
        catch
        {
            RollbackTransaction();
            throw;
        }

        foreach (var action in _postCommitActions)
        {
            try
            {
                await action();
            }
            catch
            {
                // Post-commit actions are best-effort.
            }
        }
    }

    public void RollbackTransaction() => _databaseFacade.RollbackTransaction();
}
