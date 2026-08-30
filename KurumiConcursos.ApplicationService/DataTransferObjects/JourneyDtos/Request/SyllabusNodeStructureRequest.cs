namespace KurumiConcursos.ApplicationService.DataTransferObjects.JourneyDtos.Request;

public sealed record SyllabusNodeStructureRequest(
    string Title,
    int Order,
    IReadOnlyList<SyllabusNodeStructureRequest> Children);