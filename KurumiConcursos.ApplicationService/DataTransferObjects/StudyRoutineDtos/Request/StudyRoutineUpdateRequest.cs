using KurumiConcursos.Domain.Enums;

namespace KurumiConcursos.ApplicationService.DataTransferObjects.StudyRoutineDtos.Request;

public sealed record StudyRoutineUpdateRequest(
    long Id,
    string Title,
    ERoutineKind Kind,
    StudyRoutineConfigurationRequest Configuration);