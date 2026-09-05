using System.Linq.Expressions;
using KurumiConcursos.Domain.Entities;

namespace KurumiConcursos.Infra.Interfaces.RepositoryContracts;

public interface ITimeCapsuleRepository
{
    Task<bool> SaveAsync(TimeCapsule entity);
    Task<bool> UpdateAsync(TimeCapsule entity);
    Task<bool> DeleteAsync(TimeCapsule entity);
    Task<TimeCapsule?> FindAsync(Expression<Func<TimeCapsule, bool>> predicate, bool tracking = false);
    Task<IList<TimeCapsule>> FindAllAsync(Expression<Func<TimeCapsule, bool>> predicate, bool tracking = false);
}