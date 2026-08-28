using KurumiConcursos.Domain.Handlers.ValidationHandler;

namespace KurumiConcursos.Domain.Interface;

public interface IValidate<T> where T : class
{
    Task<ValidationResponse> ValidationAsync(T entity);
    ValidationResponse Validation(T entity);
}