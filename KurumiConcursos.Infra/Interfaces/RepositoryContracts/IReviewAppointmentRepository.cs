using System.Linq.Expressions;
using KurumiConcursos.Domain.Entities;
using KurumiConcursos.Domain.Handlers.PaginationHandler;
using KurumiConcursos.Domain.Handlers.PaginationHandler.Filters;
using Microsoft.EntityFrameworkCore.Query;

namespace KurumiConcursos.Infra.Interfaces.RepositoryContracts;

public interface IReviewAppointmentRepository
{
    Task<bool> SaveAsync(ReviewAppointment appointment);
    Task<bool> UpdateAsync(ReviewAppointment appointment);
    Task<bool> DeleteAsync(ReviewAppointment appointment);
    Task<bool> ExistsAsync(Expression<Func<ReviewAppointment, bool>> predicate);

    Task<ReviewAppointment?> FindByPredicateAsync(Expression<Func<ReviewAppointment, bool>> predicate,
        Func<IQueryable<ReviewAppointment>, IIncludableQueryable<ReviewAppointment, object>>? include = null,
        bool asNoTracking = false);

    Task<PageList<ReviewAppointment>> FindAllWithPaginationAsync(PageParams pageParams,
        Expression<Func<ReviewAppointment, bool>>? predicate = null,
        Func<IQueryable<ReviewAppointment>, IIncludableQueryable<ReviewAppointment, object>>? include = null);

    Task<IList<ReviewAppointment>> FindAllAsync(Expression<Func<ReviewAppointment, bool>>? predicate = null,
        Func<IQueryable<ReviewAppointment>, IIncludableQueryable<ReviewAppointment, object>>? include = null);
}