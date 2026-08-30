using KurumiConcursos.Domain.Enums;

namespace KurumiConcursos.ApplicationService.DataTransferObjects.JourneyDtos.Response;

public sealed record JourneySummaryResponse(
    long Id,
    string Title,
    string? Institution,
    string? Position,
    DateOnly? ExamDate,
    EJourneyStage Stage,
    int KnowledgeAreas,
    string? LogoUrl);