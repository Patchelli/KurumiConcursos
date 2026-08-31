using System.Linq.Expressions;
using KurumiConcursos.Domain.Entities;
using KurumiConcursos.Domain.Handlers.PaginationHandler;
using KurumiConcursos.Domain.Handlers.PaginationHandler.Filters;
using KurumiConcursos.Infra.Interfaces.RepositoryContracts;
using KurumiConcursos.Infra.Interfaces.ServiceContracts;
using KurumiConcursos.Infra.ORM.Context;
using KurumiConcursos.Infra.Repositories.Base;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Query;

namespace KurumiConcursos.Infra.Repositories;

public sealed class StudyRoutineRepository(
    ApplicationContext dbContext,
    IPaginationQueryService<StudyRoutine> paginationQueryService)
    : RepositoryBase<StudyRoutine>(dbContext), IStudyRoutineRepository
{
    public async Task<bool> SaveAsync(StudyRoutine studyRoutine)
    {
        await DbSetContext.AddAsync(studyRoutine);
        return await SaveInDatabaseAsync();
    }

    public Task<bool> UpdateAsync(StudyRoutine studyRoutine)
    {
        DetachedObject(studyRoutine);
        DbSetContext.Update(studyRoutine);
        return SaveInDatabaseAsync();
    }

    public Task<bool> DeleteAsync(StudyRoutine studyRoutine)
    {
        DbSetContext.Remove(studyRoutine);
        return SaveInDatabaseAsync();
    }

    public Task<bool> ExistsAsync(Expression<Func<StudyRoutine, bool>> predicate) =>
        DbSetContext.AsNoTracking().AnyAsync(predicate);

    public Task<StudyRoutine?> FindByPredicateAsync(Expression<Func<StudyRoutine, bool>> predicate,
        Func<IQueryable<StudyRoutine>, IIncludableQueryable<StudyRoutine, object>>? include = null,
        bool asNoTracking = false)
    {
        IQueryable<StudyRoutine> query = DbSetContext;
        if (asNoTracking) query = query.AsNoTracking();
        if (include is not null) query = include(query);
        return query.FirstOrDefaultAsync(predicate);
    }

    public Task<PageList<StudyRoutine>> FindAllWithPaginationAsync(PageParams pageParams,
        Expression<Func<StudyRoutine, bool>>? predicate = null,
        Func<IQueryable<StudyRoutine>, IIncludableQueryable<StudyRoutine, object>>? include = null)
    {
        IQueryable<StudyRoutine> query = DbSetContext;
        if (include is not null) query = include(query);
        if (predicate is not null) query = query.Where(predicate);
        query = query.OrderByDescending(x => x.Id);
        return paginationQueryService.CreatePaginationAsync(query, pageParams.PageSize, pageParams.PageNumber);
    }

    public async Task<IList<StudyRoutine>> FindAllAsync(Expression<Func<StudyRoutine, bool>>? predicate = null,
        Func<IQueryable<StudyRoutine>, IIncludableQueryable<StudyRoutine, object>>? include = null)
    {
        IQueryable<StudyRoutine> query = DbSetContext;
        if (include is not null) query = include(query);
        if (predicate is not null) query = query.Where(predicate);
        return await query.AsNoTracking().ToListAsync();
    }
}