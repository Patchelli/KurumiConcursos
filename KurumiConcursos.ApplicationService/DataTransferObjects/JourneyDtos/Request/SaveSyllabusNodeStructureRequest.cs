namespace KurumiConcursos.ApplicationService.DataTransferObjects.JourneyDtos.Request;

public sealed record SaveSyllabusNodeStructureRequest(
    string Title,
    int Order,
    IReadOnlyList<SaveSyllabusNodeStructureRequest> Children);
