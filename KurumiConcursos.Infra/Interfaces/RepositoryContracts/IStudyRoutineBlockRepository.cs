using System.Linq.Expressions;
using KurumiConcursos.Domain.Entities;
using KurumiConcursos.Domain.Handlers.PaginationHandler;
using KurumiConcursos.Domain.Handlers.PaginationHandler.Filters;
using Microsoft.EntityFrameworkCore.Query;

namespace KurumiConcursos.Infra.Interfaces.RepositoryContracts;

public interface IStudyRoutineBlockRepository
{
    Task<bool> SaveAsync(StudyRoutineBlock block);
    Task<bool> UpdateAsync(StudyRoutineBlock block);
    Task<bool> DeleteAsync(StudyRoutineBlock block);
    Task<bool> ExistsAsync(Expression<Func<StudyRoutineBlock, bool>> predicate);

    Task<StudyRoutineBlock?> FindByPredicateAsync(Expression<Func<StudyRoutineBlock, bool>> predicate,
        Func<IQueryable<StudyRoutineBlock>, IIncludableQueryable<StudyRoutineBlock, object>>? include = null,
        bool asNoTracking = false);

    Task<PageList<StudyRoutineBlock>> FindAllWithPaginationAsync(PageParams pageParams,
        Expression<Func<StudyRoutineBlock, bool>>? predicate = null,
        Func<IQueryable<StudyRoutineBlock>, IIncludableQueryable<StudyRoutineBlock, object>>? include = null);

    Task<IList<StudyRoutineBlock>> FindAllAsync(Expression<Func<StudyRoutineBlock, bool>>? predicate = null,
        Func<IQueryable<StudyRoutineBlock>, IIncludableQueryable<StudyRoutineBlock, object>>? include = null);
}