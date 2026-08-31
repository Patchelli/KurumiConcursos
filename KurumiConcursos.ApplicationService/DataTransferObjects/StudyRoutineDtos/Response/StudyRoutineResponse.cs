using KurumiConcursos.ApplicationService.DataTransferObjects.StudyRoutineDtos.Request;
using KurumiConcursos.Domain.Enums;

namespace KurumiConcursos.ApplicationService.DataTransferObjects.StudyRoutineDtos.Response;

public sealed record StudyRoutineResponse(
    long Id,
    long JourneyId,
    string Title,
    ERoutineKind Kind,
    bool Active,
    StudyRoutineConfigurationRequest Configuration);