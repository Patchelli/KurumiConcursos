using System.Linq.Expressions;
using KurumiConcursos.Domain.Entities;

namespace KurumiConcursos.Infra.Interfaces.RepositoryContracts;

public interface IPracticeEntryRepository
{
    Task<bool> SaveAsync(PracticeEntry e);
    Task<bool> UpdateAsync(PracticeEntry e);
    Task<bool> DeleteAsync(PracticeEntry e);
    Task<PracticeEntry?> FindAsync(Expression<Func<PracticeEntry, bool>> p, bool tracking = false);
    Task<IList<PracticeEntry>> FindAllAsync(Expression<Func<PracticeEntry, bool>> p);
}