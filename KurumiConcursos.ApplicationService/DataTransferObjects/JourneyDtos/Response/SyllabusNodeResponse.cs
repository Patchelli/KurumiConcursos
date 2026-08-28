using KurumiConcursos.Domain.Enums;

namespace KurumiConcursos.ApplicationService.DataTransferObjects.JourneyDtos.Response;

public sealed record SyllabusNodeResponse(
    long Id,
    long? ParentId,
    string Title,
    int Order,
    EStudyProgress Progress,
    DateOnly? StudyStartedOn,
    DateOnly? StudiedOn,
    IReadOnlyList<SyllabusNodeResponse> Children);
