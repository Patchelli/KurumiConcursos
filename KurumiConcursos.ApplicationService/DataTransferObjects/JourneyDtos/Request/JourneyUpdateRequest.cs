using KurumiConcursos.Domain.Enums;

namespace KurumiConcursos.ApplicationService.DataTransferObjects.JourneyDtos.Request;

public sealed record JourneyUpdateRequest(
    long Id,
    string Title,
    string? Institution,
    string? ExamBoard,
    string? Position,
    decimal? Salary,
    int? Openings,
    string? NoticeUrl,
    DateOnly? ExamDate,
    EJourneyStage Stage,
    bool IncludeInStatistics,
    string? LogoUrl,
    IReadOnlyList<KnowledgeAreaStructureRequest> KnowledgeAreas);