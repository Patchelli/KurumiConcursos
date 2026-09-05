namespace KurumiConcursos.ApplicationService.DataTransferObjects.MockAssessmentDtos.Response;

public sealed record MockAssessmentBreakdownResponse(
    long KnowledgeAreaId,
    int TotalQuestions,
    int CorrectAnswers,
    int VoidedQuestions,
    IReadOnlyDictionary<string, int> ErrorReasons,
    string? Notes);

public sealed record MockAssessmentResponse(
    long Id,
    long JourneyId,
    string Title,
    string? Source,
    DateOnly AssessmentDate,
    int DurationMinutes,
    int TotalQuestions,
    int CorrectAnswers,
    decimal? Score,
    IReadOnlyList<MockAssessmentBreakdownResponse> Breakdown);