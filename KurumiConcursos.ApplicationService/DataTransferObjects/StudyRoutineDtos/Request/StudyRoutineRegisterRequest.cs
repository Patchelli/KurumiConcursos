using KurumiConcursos.Domain.Enums;

namespace KurumiConcursos.ApplicationService.DataTransferObjects.StudyRoutineDtos.Request;

public sealed record StudyRoutineRegisterRequest(
    long JourneyId,
    string Title,
    ERoutineKind Kind,
    StudyRoutineConfigurationRequest Configuration);