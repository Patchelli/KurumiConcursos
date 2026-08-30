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

public sealed class PersonalDataRepository(
    ApplicationContext dbContext,
    IPaginationQueryService<PersonalData> paginationQueryService)
    : RepositoryBase<PersonalData>(dbContext), IPersonalDataRepository
{
    public async Task<bool> SaveAsync(PersonalData personalData)
    {
        await DbSetContext.AddAsync(personalData);
        return await SaveInDatabaseAsync();
    }

    public Task<bool> UpdateAsync(PersonalData personalData)
    {
        DetachedObject(personalData);
        DbSetContext.Update(personalData);
        return SaveInDatabaseAsync();
    }

    public Task<bool> DeleteAsync(PersonalData personalData)
    {
        DbSetContext.Remove(personalData);
        return SaveInDatabaseAsync();
    }

    public Task<bool> ExistsByPredicateAsync(Expression<Func<PersonalData, bool>> predicate) =>
        DbSetContext.AnyAsync(predicate);

    public Task<PersonalData?> FindByPredicateAsync(
        Expression<Func<PersonalData, bool>> predicate,
        Func<IQueryable<PersonalData>, IIncludableQueryable<PersonalData, object>>? include = null,
        bool asNoTracking = false)
    {
        IQueryable<PersonalData> query = DbSetContext;
        if (asNoTracking) query = query.AsNoTracking();
        if (include is not null) query = include(query);
        return query.FirstOrDefaultAsync(predicate);
    }

    public Task<PageList<PersonalData>> FindAllWithPaginationAsync(
        PersonalDataPageParams pageParams,
        Expression<Func<PersonalData, bool>>? predicate = null,
        Func<IQueryable<PersonalData>, IIncludableQueryable<PersonalData, object>>? include = null)
    {
        IQueryable<PersonalData> query = DbSetContext;
        if (include is not null) query = include(query);
        if (predicate is not null) query = query.Where(predicate);
        query = query.OrderByDescending(personalData => personalData.Id);
        return paginationQueryService.CreatePaginationAsync(query, pageParams.PageSize, pageParams.PageNumber);
    }

    public Task<List<PersonalData>> FindAllByPredicateAsync(
        Expression<Func<PersonalData, bool>> predicate,
        Func<IQueryable<PersonalData>, IIncludableQueryable<PersonalData, object>>? include = null,
        bool toQuery = false)
    {
        IQueryable<PersonalData> query = DbSetContext;
        if (include is not null) query = include(query);
        if (toQuery) query = query.AsNoTracking();
        return query.Where(predicate).ToListAsync();
    }

    public async Task<IList<PersonalData>> FindAllAsync(
        Expression<Func<PersonalData, bool>>? predicate = null,
        Func<IQueryable<PersonalData>, IIncludableQueryable<PersonalData, object>>? include = null)
    {
        IQueryable<PersonalData> query = DbSetContext;
        if (include is not null) query = include(query);
        if (predicate is not null) query = query.Where(predicate);
        return await query.AsNoTracking().ToListAsync();
    }
}