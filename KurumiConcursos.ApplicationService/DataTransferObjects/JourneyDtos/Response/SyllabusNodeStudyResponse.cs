using KurumiConcursos.Domain.Enums;

namespace KurumiConcursos.ApplicationService.DataTransferObjects.JourneyDtos.Response;

public sealed record SyllabusNodeStudyResponse(
    long SyllabusNodeId,
    EStudyProgress Progress,
    DateOnly? StudyStartedOn,
    DateOnly? StudiedOn,
    int StudiedMinutes,
    DateOnly? ReviewDate);