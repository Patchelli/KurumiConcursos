using KurumiConcursos.Domain.Entities;

namespace KurumiConcursos.ApplicationService.Interfaces.ServiceContracts;

public interface ITokenService
{
    string Create(User user);
}
