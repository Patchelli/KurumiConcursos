namespace KurumiConcursos.ApplicationService.DataTransferObjects.JourneyDtos.Response;

public sealed record KnowledgeAreaResponse(
    long Id,
    string Title,
    int Order,
    decimal? Weight,
    int? ExpectedQuestions,
    IReadOnlyList<SyllabusNodeResponse> Nodes);
