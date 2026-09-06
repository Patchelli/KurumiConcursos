using System.Linq.Expressions;
using KurumiConcursos.Domain.Entities;
using KurumiConcursos.Domain.Handlers.PaginationHandler;
using KurumiConcursos.Domain.Handlers.PaginationHandler.Filters;
using Microsoft.EntityFrameworkCore.Query;

namespace KurumiConcursos.Infra.Interfaces.RepositoryContracts;

public interface IQuestionAppointmentRepository
{
    Task<bool> SaveAsync(QuestionAppointment appointment);
    Task<bool> UpdateAsync(QuestionAppointment appointment);
    Task<bool> DeleteAsync(QuestionAppointment appointment);
    Task<bool> ExistsAsync(Expression<Func<QuestionAppointment, bool>> predicate);
    Task<QuestionAppointment?> FindByPredicateAsync(
        Expression<Func<QuestionAppointment, bool>> predicate,
        Func<IQueryable<QuestionAppointment>, IIncludableQueryable<QuestionAppointment, object>>? include = null,
        bool asNoTracking = false);
    Task<PageList<QuestionAppointment>> FindAllWithPaginationAsync(
        PageParams pageParams,
        Expression<Func<QuestionAppointment, bool>>? predicate = null,
        Func<IQueryable<QuestionAppointment>, IIncludableQueryable<QuestionAppointment, object>>? include = null);
    Task<IList<QuestionAppointment>> FindAllAsync(
        Expression<Func<QuestionAppointment, bool>>? predicate = null,
        Func<IQueryable<QuestionAppointment>, IIncludableQueryable<QuestionAppointment, object>>? include = null);
}
