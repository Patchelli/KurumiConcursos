using KurumiConcursos.ApplicationService.DataTransferObjects.RadarDtos.Request;
using KurumiConcursos.Domain.ValueObjects;

namespace KurumiConcursos.ApplicationService.Interfaces.ServiceContracts;

public interface IRadarCommandService
{
    Task<bool> SavePreferencesAsync(RadarPreferencesRequest request, UserCredential credential);
}