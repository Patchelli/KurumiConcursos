using System.Linq.Expressions;
using KurumiConcursos.Domain.Entities;
using KurumiConcursos.Domain.Handlers.PaginationHandler;
using KurumiConcursos.Domain.Handlers.PaginationHandler.Filters;
using Microsoft.EntityFrameworkCore.Query;

namespace KurumiConcursos.Infra.Interfaces.RepositoryContracts;

public interface IStudyRoutineRepository
{
    Task<bool> SaveAsync(StudyRoutine studyRoutine);
    Task<bool> UpdateAsync(StudyRoutine studyRoutine);
    Task<bool> DeleteAsync(StudyRoutine studyRoutine);
    Task<bool> ExistsAsync(Expression<Func<StudyRoutine, bool>> predicate);

    Task<StudyRoutine?> FindByPredicateAsync(Expression<Func<StudyRoutine, bool>> predicate,
        Func<IQueryable<StudyRoutine>, IIncludableQueryable<StudyRoutine, object>>? include = null,
        bool asNoTracking = false);

    Task<PageList<StudyRoutine>> FindAllWithPaginationAsync(PageParams pageParams,
        Expression<Func<StudyRoutine, bool>>? predicate = null,
        Func<IQueryable<StudyRoutine>, IIncludableQueryable<StudyRoutine, object>>? include = null);

    Task<IList<StudyRoutine>> FindAllAsync(Expression<Func<StudyRoutine, bool>>? predicate = null,
        Func<IQueryable<StudyRoutine>, IIncludableQueryable<StudyRoutine, object>>? include = null);
}