using System.Linq.Expressions;
using KurumiConcursos.Domain.Entities;
using Microsoft.EntityFrameworkCore.Query;

namespace KurumiConcursos.Infra.Interfaces.RepositoryContracts;

public interface IStudyResourceRepository
{
    Task<bool> SaveAsync(StudyResource resource);
    Task<bool> UpdateAsync(StudyResource resource);
    Task<bool> DeleteAsync(StudyResource resource);

    Task<StudyResource?> FindByPredicateAsync(Expression<Func<StudyResource, bool>> predicate,
        Func<IQueryable<StudyResource>, IIncludableQueryable<StudyResource, object>>? include = null,
        bool asNoTracking = false);

    Task<IList<StudyResource>> FindAllAsync(Expression<Func<StudyResource, bool>>? predicate = null,
        Func<IQueryable<StudyResource>, IIncludableQueryable<StudyResource, object>>? include = null);
}