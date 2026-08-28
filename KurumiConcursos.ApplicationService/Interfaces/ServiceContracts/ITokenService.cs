namespace KurumiConcursos.ApplicationService.Interfaces.ServiceContracts;

public interface ITokenService
{
    string Create(Guid userId, string name, string email);
}