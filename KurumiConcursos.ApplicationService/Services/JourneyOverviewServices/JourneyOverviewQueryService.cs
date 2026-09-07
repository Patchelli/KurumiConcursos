using System.Text.Json;
using KurumiConcursos.ApplicationService.DataTransferObjects.JourneyOverviewDtos.Response;
using KurumiConcursos.ApplicationService.Interfaces.MapperContracts;
using KurumiConcursos.ApplicationService.Interfaces.ServiceContracts;
using KurumiConcursos.Domain.Entities;
using KurumiConcursos.Domain.Enums;
using KurumiConcursos.Domain.ValueObjects;
using KurumiConcursos.Infra.Interfaces.RepositoryContracts;

namespace KurumiConcursos.ApplicationService.Services.JourneyOverviewServices;

public sealed class JourneyOverviewQueryService(
    IJourneyRepository journeyRepository,
    IFocusSessionRepository focusSessionRepository,
    IStudyRoutineBlockRepository blockRepository,
    IPracticeEntryRepository practiceRepository,
    IMockAssessmentRepository mockAssessmentRepository,
    IFlashcardRepository flashcardRepository,
    IJourneyOverviewMapper mapper) : IJourneyOverviewQueryService
{
    public async Task<JourneyOverviewResponse?> FindAsync(long journeyId, UserCredential credential)
    {
        var journey = await journeyRepository.FindByIdAsync(
            journeyId, credential.UserId, CancellationToken.None, includeStructure: true);
        if (journey is null) return null;

        var sessions = await focusSessionRepository.FindAllAsync(item =>
            item.UserId == credential.UserId && item.JourneyId == journeyId);
        var blocks = await blockRepository.FindAllAsync(item =>
            item.UserId == credential.UserId && item.JourneyId == journeyId);
        var practices = await practiceRepository.FindAllAsync(item =>
            item.UserId == credential.UserId && item.JourneyId == journeyId);
        var assessments = await mockAssessmentRepository.FindAllAsync(item =>
            item.UserId == credential.UserId && item.JourneyId == journeyId);
        var cards = await flashcardRepository.FindCardsAsync(credential.UserId, journeyId, null, null);

        var allNodes = journey.KnowledgeAreas.SelectMany(area => area.SyllabusNodes).ToList();
        var rootTopics = allNodes.Where(item => !item.ParentId.HasValue).ToList();
        var subtopics = allNodes.Where(item => item.ParentId.HasValue).ToList();
        var nodeArea = allNodes.ToDictionary(node => node.Id, node => node.KnowledgeAreaId);
        var learningUnits = BuildStudyUnits(sessions, blocks, nodeArea);
        var studyUnits = learningUnits.ToList();
        studyUnits.AddRange(assessments.Where(item => item.DurationMinutes > 0)
            .Select(item => new StudyUnit(item.AssessmentDate, null, null, item.DurationMinutes)));
        var today = CurrentDate();
        var questionUnits = BuildQuestionUnits(practices, assessments);
        var totalQuestions = questionUnits.Sum(item => item.Questions);
        var totalCorrect = questionUnits.Sum(item => item.Correct);
        var completedTopics = rootTopics.Count(item => item.Progress == EStudyProgress.Studied);
        var studyDays = learningUnits.Select(item => item.Date).Distinct().Order().ToList();
        var todayQuestions = questionUnits.Where(item => item.Date == today).Sum(item => item.Questions);
        var todayCorrect = questionUnits.Where(item => item.Date == today).Sum(item => item.Correct);

        var coverage = Percentage(allNodes.Count(item => item.Progress == EStudyProgress.Studied), allNodes.Count);
        var application = Percentage(totalCorrect, totalQuestions);
        var recalls = cards.SelectMany(card => card.Recalls).ToList();
        var retention = Percentage(recalls.Count(item => item.Grade >= ERecallGrade.Good), recalls.Count);
        var elapsedBlocks = blocks.Where(item => item.ScheduledFor <= today).ToList();
        var plannedDays = elapsedBlocks.Select(item => item.ScheduledFor).Distinct().Count();
        var completedDays = elapsedBlocks.Where(item => item.CompletedMinutes > 0)
            .Select(item => item.ScheduledFor).Distinct().Count();
        var consistency = plannedDays == 0 ? 0 : Percentage(completedDays, plannedDays);
        var score = Round(coverage * .40m + application * .25m + retention * .20m + consistency * .15m);

        var summary = new JourneyOverviewSummaryResponse(
            studyUnits.Sum(item => item.Minutes), totalQuestions, totalCorrect,
            NullablePercentage(totalCorrect, totalQuestions), studyDays.Count, completedTopics, rootTopics.Count,
            subtopics.Count(item => item.Progress == EStudyProgress.Studied), subtopics.Count,
            studyUnits.Where(item => item.Date == today).Sum(item => item.Minutes), todayQuestions,
            NullablePercentage(todayCorrect, todayQuestions),
            CountTodaySessions(sessions, blocks, assessments, today), CalculateStreak(studyDays, today));
        var readiness = new JourneyOverviewReadinessResponse(
            score, Level(score), coverage, application, retention, consistency);
        var days = BuildDays(studyUnits, questionUnits);
        var weeks = BuildWeeks(questionUnits, recalls, today);
        var areas = journey.KnowledgeAreas.OrderBy(item => item.Order)
            .Select(area => BuildArea(area, studyUnits, practices, assessments, cards)).ToList();
        var errors = BuildErrors(practices, assessments);
        return mapper.DomainToDtoResponse(summary, readiness, days, weeks, areas, errors);
    }

    private static List<StudyUnit> BuildStudyUnits(IList<FocusSession> sessions,
        IList<StudyRoutineBlock> blocks, IReadOnlyDictionary<long, long> nodeArea)
    {
        var focus = sessions.GroupBy(item => new { item.StudyDate, item.SyllabusNodeId, item.KnowledgeAreaId })
            .Select(group => new StudyUnit(group.Key.StudyDate, group.Key.SyllabusNodeId,
                group.Key.KnowledgeAreaId ??
                (group.Key.SyllabusNodeId.HasValue &&
                 nodeArea.TryGetValue(group.Key.SyllabusNodeId.Value, out var areaId)
                    ? areaId
                    : null),
                (int)Math.Ceiling(group.Sum(item => item.DurationSeconds) / 60m))).ToList();
        var planned = blocks.Where(item => item.CompletedMinutes > 0)
            .GroupBy(item => new { item.ScheduledFor, item.SyllabusNodeId })
            .Select(group => new StudyUnit(group.Key.ScheduledFor, group.Key.SyllabusNodeId,
                nodeArea.TryGetValue(group.Key.SyllabusNodeId, out var areaId) ? areaId : null,
                group.Sum(item => item.CompletedMinutes))).ToList();
        return focus.Concat(planned).GroupBy(item => new { item.Date, item.NodeId })
            .Select(group => group.OrderByDescending(item => item.Minutes).First()).ToList();
    }

    private static List<QuestionUnit> BuildQuestionUnits(IList<PracticeEntry> practices,
        IList<MockAssessment> assessments)
    {
        var result = practices.Select(item => new QuestionUnit(item.PracticeDate, item.KnowledgeAreaId,
                item.SyllabusNodeId, Math.Max(0, item.QuestionsAnswered - item.VoidedQuestions), item.CorrectAnswers))
            .ToList();
        result.AddRange(assessments.Select(assessment => new QuestionUnit(
            assessment.AssessmentDate, null, null, assessment.TotalQuestions, assessment.CorrectAnswers)));

        return result;
    }

    private static IList<JourneyOverviewDayResponse> BuildDays(IList<StudyUnit> studies,
        IList<QuestionUnit> questions)
    {
        var dates = studies.Select(item => item.Date).ToList();
        var weekCount = dates.Count == 0
            ? 1
            : Math.Max(1, (int)Math.Ceiling((CurrentDate().DayNumber - dates.Min().DayNumber + 1) / 7m));
        return Enumerable.Range(0, 7).Select(day =>
        {
            var q = questions.Where(item => (int)item.Date.DayOfWeek == day).ToList();
            return new JourneyOverviewDayResponse(day,
                (int)Math.Round(studies.Where(item => (int)item.Date.DayOfWeek == day)
                    .Sum(item => item.Minutes) / (decimal)weekCount, MidpointRounding.AwayFromZero),
                q.Sum(item => item.Questions),
                NullablePercentage(q.Sum(item => item.Correct), q.Sum(item => item.Questions)));
        }).ToList();
    }

    private static IList<JourneyOverviewWeekResponse> BuildWeeks(IList<QuestionUnit> questions,
        IList<MemoryRecall> recalls, DateOnly today)
    {
        var currentMonday = today.AddDays(-(((int)today.DayOfWeek + 6) % 7));
        return Enumerable.Range(0, 8).Select(index =>
        {
            var start = currentMonday.AddDays((index - 7) * 7);
            var end = start.AddDays(7);
            var q = questions.Where(item => item.Date >= start && item.Date < end).ToList();
            var r = recalls.Where(item =>
                DateOnly.FromDateTime(item.AnsweredAt.DateTime) >= start &&
                DateOnly.FromDateTime(item.AnsweredAt.DateTime) < end).ToList();
            return new JourneyOverviewWeekResponse($"{start:dd/MM}–{end.AddDays(-1):dd/MM}",
                NullablePercentage(q.Sum(item => item.Correct), q.Sum(item => item.Questions)),
                NullablePercentage(r.Count(item => item.Grade >= ERecallGrade.Good), r.Count));
        }).ToList();
    }

    private static JourneyOverviewAreaResponse BuildArea(KnowledgeArea area, IList<StudyUnit> studies,
        IList<PracticeEntry> practices, IList<MockAssessment> assessments, IList<MemoryCard> cards)
    {
        var nodes = area.SyllabusNodes.ToList();
        var nodeIds = nodes.Select(item => item.Id).ToHashSet();
        var areaPractices = practices.Where(item => item.KnowledgeAreaId == area.Id).ToList();
        var breakdowns = assessments.SelectMany(item => item.Breakdown).Where(item => item.KnowledgeAreaId == area.Id)
            .ToList();
        var questions = areaPractices.Sum(item => Math.Max(0, item.QuestionsAnswered - item.VoidedQuestions)) +
                        breakdowns.Sum(item => Math.Max(0, item.TotalQuestions - item.VoidedQuestions));
        var correct = areaPractices.Sum(item => item.CorrectAnswers) + breakdowns.Sum(item => item.CorrectAnswers);
        var reasons = MergeReasons(areaPractices.Select(item => item.ErrorReasonsJson)
            .Concat(breakdowns.Select(item => item.ErrorReasonsJson)));
        var areaCards = cards.Where(item => item.Collection.KnowledgeAreaId == area.Id).ToList();
        var areaRecalls = areaCards.SelectMany(item => item.Recalls).ToList();
        var assessmentMinutes = assessments.Sum(assessment =>
        {
            var total = assessment.Breakdown.Sum(item => Math.Max(0, item.TotalQuestions - item.VoidedQuestions));
            var areaTotal = assessment.Breakdown.Where(item => item.KnowledgeAreaId == area.Id)
                .Sum(item => Math.Max(0, item.TotalQuestions - item.VoidedQuestions));
            return total == 0
                ? 0
                : (int)Math.Round(assessment.DurationMinutes * areaTotal / (decimal)total,
                    MidpointRounding.AwayFromZero);
        });
        var predominantReason = reasons.OrderByDescending(item => item.Value).FirstOrDefault();
        var topics = nodes.Where(item => !item.ParentId.HasValue).OrderBy(item => item.Order).Select(node =>
        {
            var descendants = Descendants(node.Id, nodes).Append(node).ToList();
            var ids = descendants.Select(item => item.Id).ToHashSet();
            var p = areaPractices.Where(item => item.SyllabusNodeId.HasValue && ids.Contains(item.SyllabusNodeId.Value))
                .ToList();
            var q = p.Sum(item => Math.Max(0, item.QuestionsAnswered - item.VoidedQuestions));
            return new JourneyOverviewTopicResponse(node.Id, node.Title,
                Percentage(descendants.Count(item => item.Progress == EStudyProgress.Studied), descendants.Count),
                NullablePercentage(p.Sum(item => item.CorrectAnswers), q));
        }).ToList();
        return new JourneyOverviewAreaResponse(area.Id, area.Title,
            studies.Where(item => item.AreaId == area.Id).Sum(item => item.Minutes) + assessmentMinutes,
            Percentage(nodes.Count(item => item.Progress == EStudyProgress.Studied), nodes.Count),
            questions, correct, NullablePercentage(correct, questions), Math.Max(0, questions - correct),
            reasons.Values.Sum(), predominantReason.Key,
            predominantReason.Value,
            NullablePercentage(predominantReason.Value, reasons.Values.Sum()),
            areaCards.Count, areaRecalls.Count,
            NullablePercentage(areaRecalls.Count(item => item.Grade >= ERecallGrade.Good), areaRecalls.Count), topics);
    }

    private static IList<JourneyOverviewErrorResponse> BuildErrors(IList<PracticeEntry> practices,
        IList<MockAssessment> assessments)
    {
        var reasons = MergeReasons(practices.Select(item => item.ErrorReasonsJson)
            .Concat(assessments.SelectMany(item => item.Breakdown).Select(item => item.ErrorReasonsJson)));
        var total = reasons.Values.Sum();
        return reasons.OrderByDescending(item => item.Value)
            .Select(item => new JourneyOverviewErrorResponse(item.Key, item.Value, Percentage(item.Value, total)))
            .ToList();
    }

    private static Dictionary<string, int> MergeReasons(IEnumerable<string> jsonValues)
    {
        var result = new Dictionary<string, int>(StringComparer.OrdinalIgnoreCase);
        foreach (var json in jsonValues)
            try
            {
                foreach (var item in JsonSerializer.Deserialize<Dictionary<string, int>>(json) ?? [])
                    if (item.Value > 0)
                        result[item.Key] = result.GetValueOrDefault(item.Key) + item.Value;
            }
            catch (JsonException)
            {
            }

        return result;
    }

    private static IEnumerable<SyllabusNode> Descendants(long parentId, IList<SyllabusNode> nodes)
    {
        foreach (var child in nodes.Where(item => item.ParentId == parentId))
        {
            yield return child;
            foreach (var nested in Descendants(child.Id, nodes)) yield return nested;
        }
    }

    private static int CalculateStreak(IList<DateOnly> dates, DateOnly today)
    {
        var set = dates.ToHashSet();
        var cursor = set.Contains(today) ? today : today.AddDays(-1);
        var count = 0;
        while (set.Contains(cursor))
        {
            count++;
            cursor = cursor.AddDays(-1);
        }

        return count;
    }

    private static int CountTodaySessions(IList<FocusSession> sessions, IList<StudyRoutineBlock> blocks,
        IList<MockAssessment> assessments, DateOnly today)
    {
        var focus = sessions.Where(item => item.StudyDate == today).ToList();
        var focusNodes = focus.Where(item => item.SyllabusNodeId.HasValue).Select(item => item.SyllabusNodeId!.Value)
            .ToHashSet();
        var manualBlocks = blocks.Count(item =>
            item.ScheduledFor == today && item.CompletedMinutes > 0 && !focusNodes.Contains(item.SyllabusNodeId));
        return focus.Count + manualBlocks +
               assessments.Count(item => item.AssessmentDate == today && item.DurationMinutes > 0);
    }

    private static decimal Percentage(int value, int total) => total <= 0 ? 0 : Round(value * 100m / total);
    private static decimal? NullablePercentage(int value, int total) => total <= 0 ? null : Percentage(value, total);

    private static decimal Round(decimal value) =>
        Math.Round(Math.Clamp(value, 0, 100), 1, MidpointRounding.AwayFromZero);

    private static string Level(decimal score) => score < 20 ? "Iniciante" :
        score < 40 ? "Básico" :
        score < 60 ? "Intermediário" :
        score < 80 ? "Avançado" : "Expert";

    private static DateOnly CurrentDate()
    {
        var zone = TimeZoneInfo.FindSystemTimeZoneById(OperatingSystem.IsWindows()
            ? "E. South America Standard Time"
            : "America/Sao_Paulo");
        return DateOnly.FromDateTime(TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow, zone));
    }

    private sealed record StudyUnit(DateOnly Date, long? NodeId, long? AreaId, int Minutes);

    private sealed record QuestionUnit(DateOnly Date, long? AreaId, long? NodeId, int Questions, int Correct);
}