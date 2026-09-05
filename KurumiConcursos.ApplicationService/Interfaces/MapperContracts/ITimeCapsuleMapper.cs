using KurumiConcursos.ApplicationService.DataTransferObjects.TimeCapsuleDtos.Request;
using KurumiConcursos.ApplicationService.DataTransferObjects.TimeCapsuleDtos.Response;
using KurumiConcursos.Domain.Entities;

namespace KurumiConcursos.ApplicationService.Interfaces.MapperContracts;

public interface ITimeCapsuleMapper
{
    TimeCapsule DtoRegisterToDomain(Guid userId, TimeCapsuleRegisterRequest dto);
    void DtoUpdateToDomain(TimeCapsuleUpdateRequest dto, TimeCapsule entity);
    TimeCapsuleResponse DomainToDtoResponse(TimeCapsule entity);
    IList<TimeCapsuleResponse> DomainToDtoResponseList(IList<TimeCapsule> entities);
}