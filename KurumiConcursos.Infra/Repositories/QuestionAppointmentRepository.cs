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

public sealed class QuestionAppointmentRepository(
    ApplicationContext dbContext,
    IPaginationQueryService<QuestionAppointment> paginationQueryService)
    : RepositoryBase<QuestionAppointment>(dbContext), IQuestionAppointmentRepository
{
    public async Task<bool> SaveAsync(QuestionAppointment appointment)
    {
        await DbSetContext.AddAsync(appointment);
        return await SaveInDatabaseAsync();
    }

    public Task<bool> UpdateAsync(QuestionAppointment appointment)
    {
        DetachedObject(appointment);
        DbSetContext.Update(appointment);
        return SaveInDatabaseAsync();
    }

    public Task<bool> DeleteAsync(QuestionAppointment appointment)
    {
        DbSetContext.Remove(appointment);
        return SaveInDatabaseAsync();
    }

    public Task<bool> ExistsAsync(Expression<Func<QuestionAppointment, bool>> predicate) =>
        DbSetContext.AsNoTracking().AnyAsync(predicate);

    public Task<QuestionAppointment?> FindByPredicateAsync(
        Expression<Func<QuestionAppointment, bool>> predicate,
        Func<IQueryable<QuestionAppointment>, IIncludableQueryable<QuestionAppointment, object>>? include = null,
        bool asNoTracking = false)
    {
        IQueryable<QuestionAppointment> query = DbSetContext;
        if (asNoTracking) query = query.AsNoTracking();
        if (include is not null) query = include(query);
        return query.FirstOrDefaultAsync(predicate);
    }

    public Task<PageList<QuestionAppointment>> FindAllWithPaginationAsync(
        PageParams pageParams,
        Expression<Func<QuestionAppointment, bool>>? predicate = null,
        Func<IQueryable<QuestionAppointment>, IIncludableQueryable<QuestionAppointment, object>>? include = null)
    {
        IQueryable<QuestionAppointment> query = DbSetContext;
        if (include is not null) query = include(query);
        if (predicate is not null) query = query.Where(predicate);
        query = query.OrderBy(item => item.ScheduledFor).ThenBy(item => item.Id);
        return paginationQueryService.CreatePaginationAsync(query, pageParams.PageSize, pageParams.PageNumber);
    }

    public async Task<IList<QuestionAppointment>> FindAllAsync(
        Expression<Func<QuestionAppointment, bool>>? predicate = null,
        Func<IQueryable<QuestionAppointment>, IIncludableQueryable<QuestionAppointment, object>>? include = null)
    {
        IQueryable<QuestionAppointment> query = DbSetContext;
        if (include is not null) query = include(query);
        if (predicate is not null) query = query.Where(predicate);
        return await query.AsNoTracking().OrderBy(item => item.ScheduledFor).ThenBy(item => item.Id).ToListAsync();
    }
}