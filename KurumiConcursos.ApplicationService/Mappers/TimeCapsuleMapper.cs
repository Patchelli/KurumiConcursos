using KurumiConcursos.ApplicationService.DataTransferObjects.TimeCapsuleDtos.Request;
using KurumiConcursos.ApplicationService.DataTransferObjects.TimeCapsuleDtos.Response;
using KurumiConcursos.ApplicationService.Interfaces.MapperContracts;
using KurumiConcursos.Domain.Entities;
using KurumiConcursos.Domain.Enums;

namespace KurumiConcursos.ApplicationService.Mappers;

public sealed class TimeCapsuleMapper : ITimeCapsuleMapper
{
    public TimeCapsule DtoRegisterToDomain(Guid userId, TimeCapsuleRegisterRequest dto)
    {
        var entity = new TimeCapsule { UserId = userId, JourneyId = dto.JourneyId };
        Apply(dto.Title, dto.Message, dto.VideoUrl, dto.TriggerType, dto.TriggerReferenceId,
            dto.TriggerReferenceLabel, dto.TriggerValue, dto.ScheduledAt, entity);
        return entity;
    }

    public void DtoUpdateToDomain(TimeCapsuleUpdateRequest dto, TimeCapsule entity) =>
        Apply(dto.Title, dto.Message, dto.VideoUrl, dto.TriggerType, dto.TriggerReferenceId,
            dto.TriggerReferenceLabel, dto.TriggerValue, dto.ScheduledAt, entity);

    public TimeCapsuleResponse DomainToDtoResponse(TimeCapsule x) => new(x.Id, x.JourneyId, x.Title,
        x.Message, x.VideoUrl, x.Status.ToString().ToUpperInvariant(), TriggerName(x.TriggerType),
        x.TriggerReferenceId, x.TriggerReferenceLabel, x.TriggerValue, x.ScheduledAt, x.CreationDate,
        x.DeliveredAt, x.OpenedAt);

    public IList<TimeCapsuleResponse> DomainToDtoResponseList(IList<TimeCapsule> entities) =>
        entities.Select(DomainToDtoResponse).ToList();

    private static void Apply(string title, string message, string? videoUrl, string triggerType,
        long? referenceId, string? referenceLabel, decimal? triggerValue, DateTimeOffset? scheduledAt,
        TimeCapsule entity)
    {
        entity.Title = title.Trim();
        entity.Message = message.Trim();
        entity.VideoUrl = videoUrl?.Trim();
        entity.TriggerType = TimeCapsuleTriggerTypeParser.Parse(triggerType);
        entity.TriggerReferenceId = referenceId;
        entity.TriggerReferenceLabel = referenceLabel?.Trim();
        entity.TriggerValue = entity.TriggerType == ECapsuleTriggerType.LevelReached
            ? LevelThreshold(referenceLabel)
            : triggerValue;
        entity.ScheduledAt = scheduledAt?.ToUniversalTime();
    }

    private static decimal LevelThreshold(string? level) => level?.ToLowerInvariant() switch
    {
        "básico" => 20, "intermediário" => 40, "avançado" => 60, "expert" => 80, _ => 0
    };

    private static string TriggerName(ECapsuleTriggerType x) => x switch
    {
        ECapsuleTriggerType.SubjectCompleted => "SUBJECT_COMPLETED",
        ECapsuleTriggerType.TopicCompleted => "TOPIC_COMPLETED",
        ECapsuleTriggerType.SubtopicCompleted => "SUBTOPIC_COMPLETED",
        ECapsuleTriggerType.StudyHoursReached => "STUDY_HOURS_REACHED",
        ECapsuleTriggerType.QuestionsReached => "QUESTIONS_REACHED",
        ECapsuleTriggerType.LevelReached => "LEVEL_REACHED",
        _ => "DATE"
    };
}