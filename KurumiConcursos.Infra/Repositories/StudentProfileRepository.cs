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

public sealed class StudentProfileRepository(
    ApplicationContext dbContext,
    IPaginationQueryService<StudentProfile> paginationQueryService)
    : RepositoryBase<StudentProfile>(dbContext), IStudentProfileRepository
{
    public async Task<bool> SaveAsync(StudentProfile student)
    {
        await DbSetContext.AddAsync(student);
        return await SaveInDatabaseAsync();
    }

    public Task<bool> UpdateAsync(StudentProfile student)
    {
        DetachedObject(student);
        DbSetContext.Update(student);
        return SaveInDatabaseAsync();
    }

    public Task<bool> DeleteAsync(StudentProfile student)
    {
        DbSetContext.Remove(student);
        return SaveInDatabaseAsync();
    }

    public Task<bool> ExistsAsync(Expression<Func<StudentProfile, bool>> predicate) =>
        DbSetContext.AsNoTracking().AnyAsync(predicate);

    public Task<StudentProfile?> FindByPredicateAsync(
        Expression<Func<StudentProfile, bool>> predicate,
        Func<IQueryable<StudentProfile>, IIncludableQueryable<StudentProfile, object>>? include = null,
        bool asNoTracking = false)
    {
        IQueryable<StudentProfile> query = DbSetContext;
        if (asNoTracking) query = query.AsNoTracking();
        if (include is not null) query = include(query);
        return query.FirstOrDefaultAsync(predicate);
    }

    public Task<PageList<StudentProfile>> FindAllWithPaginationAsync(
        StudentProfilePageParams pageParams,
        Expression<Func<StudentProfile, bool>>? predicate = null,
        Func<IQueryable<StudentProfile>, IIncludableQueryable<StudentProfile, object>>? include = null)
    {
        IQueryable<StudentProfile> query = DbSetContext;

        if (include is not null)
            query = include(query);

        if (predicate is not null)
            query = query.Where(predicate);

        query = query.OrderByDescending(profile => profile.Id);

        return paginationQueryService.CreatePaginationAsync(
            query,
            pageParams.PageSize,
            pageParams.PageNumber);
    }

    public async Task<IList<StudentProfile>> FindAllAsync(
        Expression<Func<StudentProfile, bool>>? predicate = null,
        Func<IQueryable<StudentProfile>, IIncludableQueryable<StudentProfile, object>>? include = null)
    {
        IQueryable<StudentProfile> query = DbSetContext;
        if (include is not null) query = include(query);
        if (predicate is not null) query = query.Where(predicate);
        return await query.AsNoTracking().ToListAsync();
    }
}
