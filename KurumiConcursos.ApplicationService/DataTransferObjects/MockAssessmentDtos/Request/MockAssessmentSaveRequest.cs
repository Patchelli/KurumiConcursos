namespace KurumiConcursos.ApplicationService.DataTransferObjects.MockAssessmentDtos.Request;

public sealed record MockAssessmentBreakdownRequest(
    long KnowledgeAreaId,
    int TotalQuestions,
    int CorrectAnswers,
    int VoidedQuestions,
    IReadOnlyDictionary<string, int> ErrorReasons,
    string? Notes);

public sealed record MockAssessmentSaveRequest(
    long JourneyId,
    string Title,
    string? Source,
    DateOnly AssessmentDate,
    int DurationMinutes,
    int TotalQuestions,
    int CorrectAnswers,
    IReadOnlyList<MockAssessmentBreakdownRequest> Breakdown);