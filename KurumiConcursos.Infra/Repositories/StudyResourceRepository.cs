using System.Linq.Expressions;
using KurumiConcursos.Domain.Entities;
using KurumiConcursos.Infra.Interfaces.RepositoryContracts;
using KurumiConcursos.Infra.ORM.Context;
using KurumiConcursos.Infra.Repositories.Base;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Query;

namespace KurumiConcursos.Infra.Repositories;

public sealed class StudyResourceRepository(ApplicationContext dbContext)
    : RepositoryBase<StudyResource>(dbContext), IStudyResourceRepository
{
    public async Task<bool> SaveAsync(StudyResource resource)
    {
        await DbSetContext.AddAsync(resource);
        return await SaveInDatabaseAsync();
    }

    public Task<bool> UpdateAsync(StudyResource resource)
    {
        DetachedObject(resource);
        DbSetContext.Update(resource);
        return SaveInDatabaseAsync();
    }

    public Task<bool> DeleteAsync(StudyResource resource)
    {
        DbSetContext.Remove(resource);
        return SaveInDatabaseAsync();
    }

    public Task<StudyResource?> FindByPredicateAsync(Expression<Func<StudyResource, bool>> predicate,
        Func<IQueryable<StudyResource>, IIncludableQueryable<StudyResource, object>>? include = null,
        bool asNoTracking = false)
    {
        IQueryable<StudyResource> q = DbSetContext;
        if (asNoTracking) q = q.AsNoTracking();
        if (include is not null) q = include(q);
        return q.FirstOrDefaultAsync(predicate);
    }

    public async Task<IList<StudyResource>> FindAllAsync(Expression<Func<StudyResource, bool>>? predicate = null,
        Func<IQueryable<StudyResource>, IIncludableQueryable<StudyResource, object>>? include = null)
    {
        IQueryable<StudyResource> q = DbSetContext;
        if (include is not null) q = include(q);
        if (predicate is not null) q = q.Where(predicate);
        return await q.AsNoTracking().OrderByDescending(x => x.Id).ToListAsync();
    }
}