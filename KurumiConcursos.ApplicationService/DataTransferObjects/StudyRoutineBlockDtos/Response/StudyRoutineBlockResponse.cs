using KurumiConcursos.Domain.Enums;

namespace KurumiConcursos.ApplicationService.DataTransferObjects.StudyRoutineBlockDtos.Response;

public sealed record StudyRoutineBlockResponse(
    long Id,
    long SyllabusNodeId,
    DateOnly ScheduledFor,
    EStudyBlockType Type,
    EStudyBlockStatus Status,
    int PlannedMinutes,
    int CompletedMinutes,
    int Order);