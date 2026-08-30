namespace KurumiConcursos.Domain.Interface;

public interface IUnitOfWork
{
    void BeginTransaction();
    Task CommitAsync(CancellationToken cancellationToken = default);
    void RollbackTransaction();
    void RegisterPostCommitAction(Func<Task> action);
}