using System.Linq.Expressions;
using KurumiConcursos.Domain.Entities;

namespace KurumiConcursos.Infra.Interfaces.RepositoryContracts;

public interface IStudySummaryRepository
{
    Task<bool> SaveAsync(StudySummary entity);
    Task<IList<StudySummary>> FindAllAsync(Expression<Func<StudySummary, bool>> predicate);
}