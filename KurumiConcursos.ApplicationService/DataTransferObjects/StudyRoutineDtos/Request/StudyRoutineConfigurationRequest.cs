namespace KurumiConcursos.ApplicationService.DataTransferObjects.StudyRoutineDtos.Request;

public sealed record StudyRoutineConfigurationRequest(
    IReadOnlyList<long> KnowledgeAreaIds,
    IReadOnlyDictionary<long, string> Affinity,
    decimal HoursPerTopic,
    IReadOnlyDictionary<string, decimal> Availability,
    IReadOnlyDictionary<long, decimal> AreaHoursOverride,
    IReadOnlyDictionary<long, decimal> NodeHoursOverride,
    bool AutomaticAdaptationEnabled = false,
    decimal AdaptationAccuracyThreshold = 60,
    int AdaptationMinimumQuestions = 10,
    int AdaptationFlashcardErrorThreshold = 10);