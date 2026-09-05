using System.Linq.Expressions;
using KurumiConcursos.ApplicationService.DataTransferObjects.TimeCapsuleDtos.Request;
using KurumiConcursos.ApplicationService.DataTransferObjects.TimeCapsuleDtos.Response;
using KurumiConcursos.ApplicationService.Interfaces.MapperContracts;
using KurumiConcursos.ApplicationService.Interfaces.ServiceContracts;
using KurumiConcursos.Domain.Entities;
using KurumiConcursos.Domain.Enums;
using KurumiConcursos.Domain.Interface;
using KurumiConcursos.Domain.ValueObjects;
using KurumiConcursos.Infra.Interfaces.RepositoryContracts;

namespace KurumiConcursos.ApplicationService.Services.TimeCapsuleServices;

public sealed class TimeCapsuleCommandService(
    ITimeCapsuleRepository repository,
    IJourneyRepository journeyRepository,
    IStudyRoutineBlockRepository blockRepository,
    IPracticeEntryRepository practiceRepository,
    IMockAssessmentRepository mockAssessmentRepository,
    IFlashcardRepository flashcardRepository,
    ITimeCapsuleMapper mapper,
    INotificationHandler notification) : ITimeCapsuleCommandService
{
    public async Task<TimeCapsuleResponse?> RegisterAsync(TimeCapsuleRegisterRequest request, UserCredential credential)
    {
        if (!await ValidateAsync(request.JourneyId, request.Title, request.Message, request.VideoUrl,
                request.TriggerType,
                request.TriggerReferenceId, request.TriggerValue, request.ScheduledAt, credential)) return null;
        var entity = mapper.DtoRegisterToDomain(credential.UserId, request);
        if (!await repository.SaveAsync(entity)) return null;
        if (entity.TriggerType != ECapsuleTriggerType.Date &&
            await IsTriggerReachedAsync(entity, DateTimeOffset.UtcNow))
        {
            entity.Status = ECapsuleStatus.Delivered;
            entity.DeliveredAt = DateTimeOffset.UtcNow;
            await repository.UpdateAsync(entity);
        }

        return mapper.DomainToDtoResponse(entity);
    }

    public async Task<TimeCapsuleResponse?> UpdateAsync(long id, TimeCapsuleUpdateRequest request,
        UserCredential credential)
    {
        var entity = await repository.FindAsync(item => item.Id == id && item.UserId == credential.UserId, true);
        if (entity is null)
        {
            notification.CreateNotification("Cápsula", "Cápsula não encontrada.");
            return null;
        }

        if (!await ValidateAsync(entity.JourneyId, request.Title, request.Message, request.VideoUrl,
                request.TriggerType,
                request.TriggerReferenceId, request.TriggerValue, request.ScheduledAt, credential)) return null;
        mapper.DtoUpdateToDomain(request, entity);
        entity.Status = ECapsuleStatus.Scheduled;
        entity.DeliveredAt = null;
        entity.OpenedAt = null;
        return await repository.UpdateAsync(entity) ? mapper.DomainToDtoResponse(entity) : null;
    }

    public async Task<TimeCapsuleResponse?> OpenAsync(long id, UserCredential credential)
    {
        var entity = await repository.FindAsync(item => item.Id == id && item.UserId == credential.UserId, true);
        if (entity is null || entity.Status == ECapsuleStatus.Scheduled) return null;
        if (entity.Status != ECapsuleStatus.Opened)
        {
            entity.Status = ECapsuleStatus.Opened;
            entity.OpenedAt = DateTimeOffset.UtcNow;
            await repository.UpdateAsync(entity);
        }

        return mapper.DomainToDtoResponse(entity);
    }

    public async Task<bool> DeleteAsync(long id, UserCredential credential)
    {
        var entity = await repository.FindAsync(item => item.Id == id && item.UserId == credential.UserId, true);
        return entity is not null && await repository.DeleteAsync(entity);
    }

    public Task EvaluateDateTriggersAsync() => EvaluateAsync(item =>
        item.Status == ECapsuleStatus.Scheduled && item.TriggerType == ECapsuleTriggerType.Date);

    public Task EvaluateNonDateTriggersAsync() => EvaluateAsync(item =>
        item.Status == ECapsuleStatus.Scheduled && item.TriggerType != ECapsuleTriggerType.Date);

    public Task EvaluateJourneyTriggersAsync(Guid userId, long journeyId) => EvaluateAsync(item =>
        item.Status == ECapsuleStatus.Scheduled && item.TriggerType != ECapsuleTriggerType.Date &&
        item.UserId == userId && item.JourneyId == journeyId);

    private async Task EvaluateAsync(Expression<Func<TimeCapsule, bool>> predicate)
    {
        foreach (var capsule in await repository.FindAllAsync(predicate, true))
            if (await IsTriggerReachedAsync(capsule, DateTimeOffset.UtcNow))
            {
                capsule.Status = ECapsuleStatus.Delivered;
                capsule.DeliveredAt = DateTimeOffset.UtcNow;
                await repository.UpdateAsync(capsule);
            }
    }

    private async Task<bool> IsTriggerReachedAsync(TimeCapsule capsule, DateTimeOffset now)
    {
        if (capsule.TriggerType == ECapsuleTriggerType.Date) return capsule.ScheduledAt <= now;

        var journey = await journeyRepository.FindByIdAsync(capsule.JourneyId, capsule.UserId,
            CancellationToken.None, includeStructure: true);
        if (journey is null) return false;
        var nodes = journey.KnowledgeAreas.SelectMany(area => area.SyllabusNodes).ToList();

        if (capsule.TriggerType is ECapsuleTriggerType.TopicCompleted or ECapsuleTriggerType.SubtopicCompleted)
            return nodes.Any(node => node.Id == capsule.TriggerReferenceId && node.Progress == EStudyProgress.Studied);

        if (capsule.TriggerType == ECapsuleTriggerType.SubjectCompleted)
        {
            var subjectNodes = nodes.Where(node => node.KnowledgeAreaId == capsule.TriggerReferenceId).ToList();
            return subjectNodes.Count > 0 && subjectNodes.All(node => node.Progress == EStudyProgress.Studied);
        }

        var blocks = await blockRepository.FindAllAsync(item =>
            item.UserId == capsule.UserId && item.JourneyId == capsule.JourneyId);
        if (capsule.TriggerType == ECapsuleTriggerType.StudyHoursReached)
            return blocks.Sum(item => item.CompletedMinutes) >= (capsule.TriggerValue ?? 0) * 60;

        var practices = await practiceRepository.FindAllAsync(item =>
            item.UserId == capsule.UserId && item.JourneyId == capsule.JourneyId);
        var simulations = await mockAssessmentRepository.FindAllAsync(item =>
            item.UserId == capsule.UserId && item.JourneyId == capsule.JourneyId);
        if (capsule.TriggerType == ECapsuleTriggerType.QuestionsReached)
            return practices.Sum(item => item.QuestionsAnswered) + simulations.Sum(item => item.TotalQuestions) >=
                   capsule.TriggerValue;

        if (capsule.TriggerType != ECapsuleTriggerType.LevelReached) return false;
        var coverage = nodes.Count == 0
            ? 0
            : nodes.Count(node => node.Progress == EStudyProgress.Studied) * 100d / nodes.Count;
        var answered = practices.Sum(item => item.QuestionsAnswered - item.VoidedQuestions) +
                       simulations.Sum(item => item.TotalQuestions);
        var application = answered == 0
            ? 0
            : (practices.Sum(item => item.CorrectAnswers) + simulations.Sum(item => item.CorrectAnswers)) * 100d /
              answered;
        var cards = await flashcardRepository.FindCardsAsync(capsule.UserId, capsule.JourneyId, null, null);
        var recalls = cards.SelectMany(card => card.Recalls).ToList();
        var retention = recalls.Count == 0
            ? 0
            : recalls.Count(item => item.Grade >= ERecallGrade.Good) * 100d / recalls.Count;
        var elapsedBlocks = blocks.Where(item => item.ScheduledFor <= DateOnly.FromDateTime(now.LocalDateTime))
            .ToList();
        var plannedDays = elapsedBlocks.Select(item => item.ScheduledFor).Distinct().Count();
        var completedDays = elapsedBlocks.Where(item => item.CompletedMinutes > 0).Select(item => item.ScheduledFor)
            .Distinct().Count();
        var consistency = plannedDays == 0 ? 0 : completedDays * 100d / plannedDays;
        var score = coverage * .40 + application * .25 + retention * .20 + consistency * .15;
        return score >= (double)(capsule.TriggerValue ?? 0);
    }

    private async Task<bool> ValidateAsync(long journeyId, string title, string message, string? videoUrl,
        string triggerName, long? referenceId, decimal? value, DateTimeOffset? scheduledAt, UserCredential credential)
    {
        if (!TimeCapsuleTriggerTypeParser.TryParse(triggerName, out var trigger) || string.IsNullOrWhiteSpace(title) ||
            title.Trim().Length > 100 || string.IsNullOrWhiteSpace(message) && string.IsNullOrWhiteSpace(videoUrl))
            return notification.CreateNotification("Cápsula", "Confira os dados informados.");
        var journey = await journeyRepository.FindByIdAsync(journeyId, credential.UserId, CancellationToken.None, true);
        if (journey is null) return notification.CreateNotification("Cápsula", "Jornada não encontrada.");
        if (trigger == ECapsuleTriggerType.Date && (!scheduledAt.HasValue || scheduledAt <= DateTimeOffset.UtcNow) ||
            trigger is ECapsuleTriggerType.SubjectCompleted or ECapsuleTriggerType.TopicCompleted
                or ECapsuleTriggerType.SubtopicCompleted && !referenceId.HasValue ||
            trigger is ECapsuleTriggerType.StudyHoursReached or ECapsuleTriggerType.QuestionsReached && value <= 0)
            return notification.CreateNotification("Cápsula", "Configure corretamente o momento da entrega.");
        if (trigger == ECapsuleTriggerType.SubjectCompleted && !journey.KnowledgeAreas.Any(x => x.Id == referenceId) ||
            trigger is ECapsuleTriggerType.TopicCompleted or ECapsuleTriggerType.SubtopicCompleted &&
            !journey.KnowledgeAreas.SelectMany(x => x.SyllabusNodes).Any(x => x.Id == referenceId))
            return notification.CreateNotification("Cápsula", "O conteúdo selecionado não pertence a esta jornada.");
        return true;
    }
}