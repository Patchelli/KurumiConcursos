namespace KurumiConcursos.ApplicationService.DataTransferObjects.StudyRoutineDtos.Request;

public sealed record StudyRoutineConfigurationRequest(
    IReadOnlyList<long> KnowledgeAreaIds,
    IReadOnlyDictionary<long, string> Affinity,
    decimal HoursPerTopic,
    int ReviewIntervalDays,
    int StudyPercentage,
    int ReviewPercentage,
    int QuestionsPercentage,
    IReadOnlyDictionary<string, decimal> Availability,
    IReadOnlyDictionary<long, decimal> AreaHoursOverride,
    IReadOnlyDictionary<long, decimal> NodeHoursOverride);