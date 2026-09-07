using System.Text.Json;
using KurumiConcursos.ApplicationService.DataTransferObjects.StudyRoutineDtos.Request;
using KurumiConcursos.ApplicationService.Interfaces.MapperContracts;
using KurumiConcursos.ApplicationService.Interfaces.ServiceContracts;
using KurumiConcursos.Domain.Entities;
using KurumiConcursos.Domain.Enums;
using KurumiConcursos.Infra.Interfaces.RepositoryContracts;

namespace KurumiConcursos.ApplicationService.Services.PerformanceAdaptationServices;

public sealed class PerformanceAdaptationCommandService(
    IStudyRoutineRepository studyRoutineRepository,
    IJourneyRepository journeyRepository,
    IPracticeEntryRepository practiceEntryRepository,
    IFlashcardRepository flashcardRepository,
    IReviewAppointmentRepository reviewAppointmentRepository,
    IQuestionAppointmentRepository questionAppointmentRepository,
    IPerformanceAdaptationMapper mapper) : IPerformanceAdaptationCommandService
{
    private static readonly JsonSerializerOptions JsonOptions = new(JsonSerializerDefaults.Web);

    public async Task EvaluateQuestionsAsync(Guid userId, long journeyId, long syllabusNodeId)
    {
        var configuration = await FindConfigurationAsync(userId, journeyId);
        if (configuration is null)
            return;

        var cycleStart = await FindCycleStartAsync(userId, syllabusNodeId, EAdaptationTrigger.Questions);
        if (!await IsCompletedNodeAsync(userId, journeyId, syllabusNodeId))
        {
            await CancelPendingAsync(userId, syllabusNodeId, EAdaptationTrigger.Questions);
            return;
        }

        var entries = await practiceEntryRepository.FindAllAsync(item =>
            item.UserId == userId && item.JourneyId == journeyId &&
            item.SyllabusNodeId == syllabusNodeId && item.CreationDate > cycleStart.StartedAt);
        var answered = entries.Sum(item => Math.Max(0, item.QuestionsAnswered - item.VoidedQuestions));
        var minimum = configuration.AdaptationMinimumQuestions > 0
            ? configuration.AdaptationMinimumQuestions
            : 10;
        var threshold = configuration.AdaptationAccuracyThreshold > 0
            ? configuration.AdaptationAccuracyThreshold
            : 60;
        var reached = answered >= minimum &&
                      entries.Sum(item => item.CorrectAnswers) * 100m / answered < threshold;
        if (cycleStart.Pending)
        {
            if (!reached)
                await CancelPendingAsync(userId, syllabusNodeId, EAdaptationTrigger.Questions);
            return;
        }

        if (reached)
            await ScheduleAsync(userId, journeyId, syllabusNodeId, EAdaptationTrigger.Questions);
    }

    public async Task EvaluateFlashcardsAsync(Guid userId, long journeyId, long syllabusNodeId)
    {
        var configuration = await FindConfigurationAsync(userId, journeyId);
        if (configuration is null || !await IsCompletedNodeAsync(userId, journeyId, syllabusNodeId))
            return;

        var cycleStart = await FindCycleStartAsync(userId, syllabusNodeId, EAdaptationTrigger.Flashcards);
        if (cycleStart.Pending)
            return;

        var cards = await flashcardRepository.FindCardsAsync(userId, journeyId, null, [syllabusNodeId]);
        var errors = cards.SelectMany(item => item.Recalls).Count(item =>
            item.Grade == ERecallGrade.Again && item.AnsweredAt > cycleStart.StartedAt);
        var threshold = configuration.AdaptationFlashcardErrorThreshold > 0
            ? configuration.AdaptationFlashcardErrorThreshold
            : 10;
        if (errors >= threshold)
            await ScheduleAsync(userId, journeyId, syllabusNodeId, EAdaptationTrigger.Flashcards);
    }

    private async Task<StudyRoutineConfigurationRequest?> FindConfigurationAsync(Guid userId, long journeyId)
    {
        var routine = await studyRoutineRepository.FindByPredicateAsync(item =>
            item.UserId == userId && item.JourneyId == journeyId && item.Active, asNoTracking: true);
        if (routine is null)
            return null;
        var configuration = JsonSerializer.Deserialize<StudyRoutineConfigurationRequest>(
            routine.ConfigurationJson, JsonOptions);
        return configuration?.AutomaticAdaptationEnabled == true ? configuration : null;
    }

    private async Task<(bool Pending, DateTimeOffset StartedAt)> FindCycleStartAsync(
        Guid userId, long syllabusNodeId, EAdaptationTrigger trigger)
    {
        var reviews = await reviewAppointmentRepository.FindAllAsync(item =>
            item.UserId == userId && item.SyllabusNodeId == syllabusNodeId && item.AdaptationTrigger == trigger);
        var questions = await questionAppointmentRepository.FindAllAsync(item =>
            item.UserId == userId && item.SyllabusNodeId == syllabusNodeId && item.AdaptationTrigger == trigger);
        var pending = reviews.Any(item => !item.Completed && !item.Superseded) ||
                      questions.Any(item => !item.Completed && !item.Superseded);
        var boundary = reviews.Where(item => item.Completed || item.Superseded).Select(CycleBoundary)
            .Concat(questions.Where(item => item.Completed || item.Superseded).Select(CycleBoundary))
            .DefaultIfEmpty(DateTimeOffset.MinValue).Max();
        return (pending, boundary);
    }

    private async Task<bool> IsCompletedNodeAsync(Guid userId, long journeyId, long syllabusNodeId)
    {
        var journey = await journeyRepository.FindByIdAsync(
            journeyId, userId, CancellationToken.None, includeStructure: true);
        return journey?.KnowledgeAreas.SelectMany(item => item.SyllabusNodes)
            .Any(item => item.Id == syllabusNodeId && item.Progress == EStudyProgress.Studied) == true;
    }

    private async Task CancelPendingAsync(Guid userId, long syllabusNodeId, EAdaptationTrigger trigger)
    {
        var reviews = await reviewAppointmentRepository.FindAllAsync(item =>
            item.UserId == userId && item.SyllabusNodeId == syllabusNodeId &&
            item.AdaptationTrigger == trigger && !item.Completed && !item.Superseded);
        foreach (var appointment in reviews)
        {
            appointment.Superseded = true;
            appointment.LastUpdateDate = DateTimeOffset.UtcNow;
            await reviewAppointmentRepository.UpdateAsync(appointment);
        }

        var questions = await questionAppointmentRepository.FindAllAsync(item =>
            item.UserId == userId && item.SyllabusNodeId == syllabusNodeId &&
            item.AdaptationTrigger == trigger && !item.Completed && !item.Superseded);
        foreach (var appointment in questions)
        {
            appointment.Superseded = true;
            appointment.LastUpdateDate = DateTimeOffset.UtcNow;
            await questionAppointmentRepository.UpdateAsync(appointment);
        }
    }

    private async Task ScheduleAsync(Guid userId, long journeyId, long syllabusNodeId,
        EAdaptationTrigger trigger)
    {
        var today = CurrentDate();
        var normalQuestions = await questionAppointmentRepository.FindAllAsync(item =>
            item.UserId == userId && item.JourneyId == journeyId && item.SyllabusNodeId == syllabusNodeId &&
            !item.Completed && !item.Superseded && item.AdaptationTrigger == null);
        foreach (var appointment in normalQuestions)
        {
            appointment.Superseded = true;
            appointment.LastUpdateDate = DateTimeOffset.UtcNow;
            await questionAppointmentRepository.UpdateAsync(appointment);
        }

        await reviewAppointmentRepository.SaveAsync(mapper.DomainToReviewAppointment(
            userId, syllabusNodeId, today.AddDays(1), trigger));
        await questionAppointmentRepository.SaveAsync(mapper.DomainToQuestionAppointment(
            userId, journeyId, syllabusNodeId, today.AddDays(2), trigger));
    }

    private static DateTimeOffset CycleBoundary(ReviewAppointment item) =>
        item.CompletedAt ?? item.LastUpdateDate ?? item.CreationDate;

    private static DateTimeOffset CycleBoundary(QuestionAppointment item) =>
        item.CompletedAt ?? item.LastUpdateDate ?? item.CreationDate;

    private static DateOnly CurrentDate()
    {
        var zone = TimeZoneInfo.FindSystemTimeZoneById(
            OperatingSystem.IsWindows() ? "E. South America Standard Time" : "America/Sao_Paulo");
        return DateOnly.FromDateTime(TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow, zone));
    }
}