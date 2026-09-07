using KurumiConcursos.ApplicationService.DataTransferObjects.RadarDtos.Request;
using KurumiConcursos.ApplicationService.DataTransferObjects.RadarDtos.Response;
using KurumiConcursos.Domain.Entities;
using KurumiConcursos.Domain.ValueObjects;

namespace KurumiConcursos.ApplicationService.Interfaces.MapperContracts;

public interface IRadarMapper
{
    void DtoUpdateToDomain(User user, RadarPreferencesRequest request);
    RadarPreferencesResponse DomainToPreferencesResponse(User user);
    RadarResultResponse DomainToResultResponse(ContestFeed feed);
}