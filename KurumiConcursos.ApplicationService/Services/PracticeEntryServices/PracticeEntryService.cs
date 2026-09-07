using System.Text.Json;
using KurumiConcursos.ApplicationService.DataTransferObjects.PracticeEntryDtos;
using KurumiConcursos.ApplicationService.Interfaces.ServiceContracts;
using KurumiConcursos.Domain.Entities;
using KurumiConcursos.Domain.Interface;
using KurumiConcursos.Domain.ValueObjects;
using KurumiConcursos.Infra.Interfaces.RepositoryContracts;

namespace KurumiConcursos.ApplicationService.Services.PracticeEntryServices;

public sealed class PracticeEntryService(
    IPracticeEntryRepository repository,
    IQuestionAppointmentCommandService questionAppointmentCommandService,
    IPerformanceAdaptationCommandService performanceAdaptationCommandService,
    IJourneyRepository journeys,
    ITimeCapsuleCommandService timeCapsuleCommandService,
    INotificationHandler notification) : IPracticeEntryService
{
    public async Task<IList<PracticeEntryResponse>> FindAllAsync(long j, long a, long? n, UserCredential c) =>
        (await repository.FindAllAsync(x =>
            x.UserId == c.UserId && x.JourneyId == j && x.KnowledgeAreaId == a &&
            (!n.HasValue || x.SyllabusNodeId == n))).Select(Map).ToList();

    public async Task<PracticeEntryResponse?> SaveAsync(long? id, PracticeEntrySaveRequest r, UserCredential c)
    {
        if (r.QuestionsAnswered <= 0 || r.CorrectAnswers < 0 || r.VoidedQuestions < 0 ||
            r.CorrectAnswers + r.VoidedQuestions > r.QuestionsAnswered)
        {
            notification.CreateNotification("Registro de questões", "Confira as quantidades informadas.");
            return null;
        }

        var j = await journeys.FindByIdAsync(r.JourneyId, c.UserId, CancellationToken.None, true);
        var a = j?.KnowledgeAreas.FirstOrDefault(x => x.Id == r.KnowledgeAreaId);
        if (a is null || r.SyllabusNodeId.HasValue && !a.SyllabusNodes.Any(x => x.Id == r.SyllabusNodeId))
        {
            notification.CreateNotification("Registro de questões", "Conteúdo não encontrado.");
            return null;
        }

        var e = id.HasValue
            ? await repository.FindAsync(x => x.Id == id && x.UserId == c.UserId, true)
            : new PracticeEntry { UserId = c.UserId };
        if (e is null)
        {
            notification.CreateNotification("Registro de questões", "Registro não encontrado.");
            return null;
        }

        e.JourneyId = r.JourneyId;
        e.KnowledgeAreaId = r.KnowledgeAreaId;
        e.SyllabusNodeId = r.SyllabusNodeId;
        e.PracticeDate = r.PracticeDate;
        e.QuestionsAnswered = r.QuestionsAnswered;
        e.CorrectAnswers = r.CorrectAnswers;
        e.VoidedQuestions = r.VoidedQuestions;
        e.ErrorReasonsJson = JsonSerializer.Serialize(r.ErrorReasons);
        e.Notes = r.Notes?.Trim();
        var ok = id.HasValue ? await repository.UpdateAsync(e) : await repository.SaveAsync(e);
        if (!ok)
        {
            notification.CreateNotification("Registro de questões", "Não foi possível salvar o registro.");
            return null;
        }

        if (r.SyllabusNodeId.HasValue)
        {
            if (!id.HasValue)
                await questionAppointmentCommandService.CompletePendingAsync(
                    c.UserId, r.JourneyId, r.SyllabusNodeId.Value);
            await performanceAdaptationCommandService.EvaluateQuestionsAsync(
                c.UserId, r.JourneyId, r.SyllabusNodeId.Value);
        }

        await timeCapsuleCommandService.EvaluateJourneyTriggersAsync(c.UserId, r.JourneyId);
        return Map(e);
    }

    public async Task<bool> DeleteAsync(long id, UserCredential c)
    {
        var e = await repository.FindAsync(x => x.Id == id && x.UserId == c.UserId, true);
        if (e is null || !await repository.DeleteAsync(e))
            return false;
        if (e.SyllabusNodeId.HasValue)
            await performanceAdaptationCommandService.EvaluateQuestionsAsync(
                c.UserId, e.JourneyId, e.SyllabusNodeId.Value);
        return true;
    }

    private static PracticeEntryResponse Map(PracticeEntry e) => new(e.Id, e.JourneyId, e.KnowledgeAreaId!.Value,
        e.SyllabusNodeId, e.PracticeDate, e.QuestionsAnswered, e.CorrectAnswers, e.VoidedQuestions,
        JsonSerializer.Deserialize<Dictionary<string, int>>(e.ErrorReasonsJson) ?? [], e.Notes);
}