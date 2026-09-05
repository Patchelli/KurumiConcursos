namespace KurumiConcursos.Domain.Enums;

public enum ECapsuleTriggerType
{
    Date = 1,
    SubjectCompleted = 2,
    TopicCompleted = 3,
    SubtopicCompleted = 4,
    StudyHoursReached = 5,
    QuestionsReached = 6,
    LevelReached = 7
}

public enum ECapsuleStatus
{
    Scheduled = 1,
    Delivered = 2,
    Opened = 3
}