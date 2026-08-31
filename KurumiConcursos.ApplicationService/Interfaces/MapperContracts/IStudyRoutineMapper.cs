using KurumiConcursos.ApplicationService.DataTransferObjects.StudyRoutineDtos.Request;
using KurumiConcursos.ApplicationService.DataTransferObjects.StudyRoutineDtos.Response;
using KurumiConcursos.Domain.Entities;

namespace KurumiConcursos.ApplicationService.Interfaces.MapperContracts;

public interface IStudyRoutineMapper
{
    StudyRoutine DtoRegisterToDomain(Guid userId, StudyRoutineRegisterRequest dto);
    StudyRoutine DtoUpdateToDomain(StudyRoutine entity, StudyRoutineUpdateRequest dto);
    StudyRoutineResponse DomainToDtoResponse(StudyRoutine entity);
}