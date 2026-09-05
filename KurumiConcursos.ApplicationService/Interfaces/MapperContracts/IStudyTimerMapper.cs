using KurumiConcursos.ApplicationService.DataTransferObjects.StudyTimerDtos.Request;
using KurumiConcursos.ApplicationService.DataTransferObjects.StudyTimerDtos.Response;
using KurumiConcursos.Domain.Entities;

namespace KurumiConcursos.ApplicationService.Interfaces.MapperContracts;

public interface IStudyTimerMapper
{
    StudyTimerSession DtoSaveToDomain(Guid userId, StudyTimerSaveRequest request, DateTimeOffset now);
    void DtoSaveToDomain(StudyTimerSaveRequest request, StudyTimerSession session, DateTimeOffset now);
    StudyTimerResponse DomainToDtoResponse(StudyTimerSession session, DateTimeOffset now);
}
