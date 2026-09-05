namespace KurumiConcursos.ApplicationService.DataTransferObjects.PracticeEntryDtos;

public sealed record PracticeEntrySaveRequest(
    long JourneyId,
    long KnowledgeAreaId,
    long? SyllabusNodeId,
    DateOnly PracticeDate,
    int QuestionsAnswered,
    int CorrectAnswers,
    int VoidedQuestions,
    IReadOnlyDictionary<string, int> ErrorReasons,
    string? Notes);

public sealed record PracticeEntryResponse(
    long Id,
    long JourneyId,
    long KnowledgeAreaId,
    long? SyllabusNodeId,
    DateOnly PracticeDate,
    int QuestionsAnswered,
    int CorrectAnswers,
    int VoidedQuestions,
    IReadOnlyDictionary<string, int> ErrorReasons,
    string? Notes);