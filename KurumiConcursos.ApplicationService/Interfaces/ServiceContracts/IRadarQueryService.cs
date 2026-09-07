using KurumiConcursos.ApplicationService.DataTransferObjects.RadarDtos.Response;
using KurumiConcursos.Domain.ValueObjects;

namespace KurumiConcursos.ApplicationService.Interfaces.ServiceContracts;

public interface IRadarQueryService
{
    Task<RadarPreferencesResponse?> GetPreferencesAsync(UserCredential credential);
    Task<RadarResultResponse?> GetContestsAsync(UserCredential credential, CancellationToken cancellationToken);
}