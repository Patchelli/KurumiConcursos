using System.Linq.Expressions;
using KurumiConcursos.Domain.Entities;

namespace KurumiConcursos.Infra.Interfaces.RepositoryContracts;

public interface IMockAssessmentRepository
{
    Task<bool> SaveAsync(MockAssessment entity);
    Task<bool> UpdateAsync(MockAssessment entity);
    Task<bool> DeleteAsync(MockAssessment entity);
    Task<MockAssessment?> FindAsync(Expression<Func<MockAssessment, bool>> predicate, bool tracking = false);
    Task<IList<MockAssessment>> FindAllAsync(Expression<Func<MockAssessment, bool>> predicate);
}