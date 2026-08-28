using System.Linq.Expressions;
using KurumiConcursos.Domain.Entities;
using KurumiConcursos.Domain.Handlers.PaginationHandler;
using KurumiConcursos.Domain.Handlers.PaginationHandler.Filters;
using Microsoft.EntityFrameworkCore.Query;

namespace KurumiConcursos.Infra.Interfaces.RepositoryContracts;

public interface IStudentProfileRepository
{
    Task<bool> SaveAsync(StudentProfile student);
    Task<bool> UpdateAsync(StudentProfile student);
    Task<bool> DeleteAsync(StudentProfile student);
    Task<bool> ExistsAsync(Expression<Func<StudentProfile, bool>> predicate);
    Task<StudentProfile?> FindByPredicateAsync(
        Expression<Func<StudentProfile, bool>> predicate,
        Func<IQueryable<StudentProfile>, IIncludableQueryable<StudentProfile, object>>? include = null,
        bool asNoTracking = false);
    Task<PageList<StudentProfile>> FindAllWithPaginationAsync(
        StudentProfilePageParams pageParams,
        Expression<Func<StudentProfile, bool>>? predicate = null,
        Func<IQueryable<StudentProfile>, IIncludableQueryable<StudentProfile, object>>? include = null);
    Task<IList<StudentProfile>> FindAllAsync(
        Expression<Func<StudentProfile, bool>>? predicate = null,
        Func<IQueryable<StudentProfile>, IIncludableQueryable<StudentProfile, object>>? include = null);
}
