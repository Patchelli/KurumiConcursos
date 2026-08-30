using KurumiConcursos.Domain.Handlers.PaginationHandler;

namespace KurumiConcursos.Infra.Interfaces.ServiceContracts;

public interface IPaginationQueryService<T> where T : class
{
    Task<PageList<T>> CreatePaginationAsync(IQueryable<T> source, int pageSize, int pageNumber);
    PageList<T> CreatePagination(List<T> source, int pageSize, int pageNumber);
}