namespace KurumiConcursos.Domain.Enums;

public static class TimeCapsuleTriggerTypeParser
{
    public static bool TryParse(string? value, out ECapsuleTriggerType triggerType)
    {
        triggerType = value?.Trim().ToUpperInvariant() switch
        {
            "DATE" => ECapsuleTriggerType.Date,
            "SUBJECT_COMPLETED" => ECapsuleTriggerType.SubjectCompleted,
            "TOPIC_COMPLETED" => ECapsuleTriggerType.TopicCompleted,
            "SUBTOPIC_COMPLETED" => ECapsuleTriggerType.SubtopicCompleted,
            "STUDY_HOURS_REACHED" => ECapsuleTriggerType.StudyHoursReached,
            "QUESTIONS_REACHED" => ECapsuleTriggerType.QuestionsReached,
            "LEVEL_REACHED" => ECapsuleTriggerType.LevelReached,
            _ => 0
        };
        return triggerType != 0;
    }

    public static ECapsuleTriggerType Parse(string value) => TryParse(value, out var triggerType)
        ? triggerType
        : throw new ArgumentException("Tipo de gatilho inválido.", nameof(value));
}