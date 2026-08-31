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

public sealed class StudyRoutineBlockRepository(
    ApplicationContext dbContext,
    IPaginationQueryService<StudyRoutineBlock> paginationQueryService)
    : RepositoryBase<StudyRoutineBlock>(dbContext), IStudyRoutineBlockRepository
{
    public async Task<bool> SaveAsync(StudyRoutineBlock block)
    {
        await DbSetContext.AddAsync(block);
        return await SaveInDatabaseAsync();
    }

    public Task<bool> UpdateAsync(StudyRoutineBlock block)
    {
        DetachedObject(block);
        DbSetContext.Update(block);
        return SaveInDatabaseAsync();
    }

    public Task<bool> DeleteAsync(StudyRoutineBlock block)
    {
        DbSetContext.Remove(block);
        return SaveInDatabaseAsync();
    }

    public Task<bool> ExistsAsync(Expression<Func<StudyRoutineBlock, bool>> predicate) =>
        DbSetContext.AsNoTracking().AnyAsync(predicate);

    public Task<StudyRoutineBlock?> FindByPredicateAsync(Expression<Func<StudyRoutineBlock, bool>> predicate,
        Func<IQueryable<StudyRoutineBlock>, IIncludableQueryable<StudyRoutineBlock, object>>? include = null,
        bool asNoTracking = false)
    {
        IQueryable<StudyRoutineBlock> query = DbSetContext;
        if (asNoTracking) query = query.AsNoTracking();
        if (include is not null) query = include(query);
        return query.FirstOrDefaultAsync(predicate);
    }

    public Task<PageList<StudyRoutineBlock>> FindAllWithPaginationAsync(PageParams pageParams,
        Expression<Func<StudyRoutineBlock, bool>>? predicate = null,
        Func<IQueryable<StudyRoutineBlock>, IIncludableQueryable<StudyRoutineBlock, object>>? include = null)
    {
        IQueryable<StudyRoutineBlock> query = DbSetContext;
        if (include is not null) query = include(query);
        if (predicate is not null) query = query.Where(predicate);
        query = query.OrderBy(x => x.ScheduledFor).ThenBy(x => x.Order);
        return paginationQueryService.CreatePaginationAsync(query, pageParams.PageSize, pageParams.PageNumber);
    }

    public async Task<IList<StudyRoutineBlock>> FindAllAsync(
        Expression<Func<StudyRoutineBlock, bool>>? predicate = null,
        Func<IQueryable<StudyRoutineBlock>, IIncludableQueryable<StudyRoutineBlock, object>>? include = null)
    {
        IQueryable<StudyRoutineBlock> query = DbSetContext;
        if (include is not null) query = include(query);
        if (predicate is not null) query = query.Where(predicate);
        return await query.AsNoTracking().OrderBy(x => x.ScheduledFor).ThenBy(x => x.Order).ToListAsync();
    }
}