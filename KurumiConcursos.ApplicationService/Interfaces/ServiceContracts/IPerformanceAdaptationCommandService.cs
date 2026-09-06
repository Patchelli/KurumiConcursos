namespace KurumiConcursos.ApplicationService.Interfaces.ServiceContracts;

public interface IPerformanceAdaptationCommandService
{
    Task EvaluateQuestionsAsync(Guid userId, long journeyId, long syllabusNodeId);
    Task EvaluateFlashcardsAsync(Guid userId, long journeyId, long syllabusNodeId);
}
