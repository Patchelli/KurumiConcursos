using System.Linq.Expressions;
using KurumiConcursos.Domain.Entities;
using KurumiConcursos.Domain.Handlers.PaginationHandler;
using KurumiConcursos.Domain.Handlers.PaginationHandler.Filters;
using Microsoft.EntityFrameworkCore.Query;

namespace KurumiConcursos.Infra.Interfaces.RepositoryContracts;

public interface IPersonalDataRepository : IDisposable
{
    Task<bool> SaveAsync(PersonalData personalData);
    Task<bool> UpdateAsync(PersonalData personalData);
    Task<bool> DeleteAsync(PersonalData personalData);
    Task<bool> ExistsByPredicateAsync(Expression<Func<PersonalData, bool>> predicate);

    Task<PersonalData?> FindByPredicateAsync(
        Expression<Func<PersonalData, bool>> predicate,
        Func<IQueryable<PersonalData>, IIncludableQueryable<PersonalData, object>>? include = null,
        bool asNoTracking = false);

    Task<PageList<PersonalData>> FindAllWithPaginationAsync(
        PersonalDataPageParams pageParams,
        Expression<Func<PersonalData, bool>>? predicate = null,
        Func<IQueryable<PersonalData>, IIncludableQueryable<PersonalData, object>>? include = null);

    Task<List<PersonalData>> FindAllByPredicateAsync(
        Expression<Func<PersonalData, bool>> predicate,
        Func<IQueryable<PersonalData>, IIncludableQueryable<PersonalData, object>>? include = null,
        bool toQuery = false);

    Task<IList<PersonalData>> FindAllAsync(
        Expression<Func<PersonalData, bool>>? predicate = null,
        Func<IQueryable<PersonalData>, IIncludableQueryable<PersonalData, object>>? include = null);
}
