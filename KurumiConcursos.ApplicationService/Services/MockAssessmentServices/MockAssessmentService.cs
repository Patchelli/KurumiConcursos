using System.Text.Json;
using KurumiConcursos.ApplicationService.DataTransferObjects.MockAssessmentDtos.Request;
using KurumiConcursos.ApplicationService.DataTransferObjects.MockAssessmentDtos.Response;
using KurumiConcursos.ApplicationService.Interfaces.ServiceContracts;
using KurumiConcursos.Domain.Entities;
using KurumiConcursos.Domain.Interface;
using KurumiConcursos.Domain.ValueObjects;
using KurumiConcursos.Infra.Interfaces.RepositoryContracts;

namespace KurumiConcursos.ApplicationService.Services.MockAssessmentServices;

public sealed class MockAssessmentService(
    IMockAssessmentRepository repository,
    IJourneyRepository journeys,
    ITimeCapsuleCommandService timeCapsuleCommandService,
    INotificationHandler notification) : IMockAssessmentService
{
    public async Task<IList<MockAssessmentResponse>> FindAllAsync(long journeyId, UserCredential c) =>
        (await repository.FindAllAsync(x => x.JourneyId == journeyId && x.UserId == c.UserId)).Select(ToResponse)
        .ToList();

    public async Task<MockAssessmentResponse?> RegisterAsync(MockAssessmentSaveRequest r, UserCredential c)
    {
        if (!await Valid(r, c)) return null;
        var e = new MockAssessment { UserId = c.UserId };
        Map(r, e);
        if (!await repository.SaveAsync(e))
        {
            notification.CreateNotification("Cadastro de simulado", "Não foi possível salvar o simulado.");
            return null;
        }

        await timeCapsuleCommandService.EvaluateJourneyTriggersAsync(c.UserId, r.JourneyId);
        return ToResponse(e);
    }

    public async Task<MockAssessmentResponse?> UpdateAsync(long id, MockAssessmentSaveRequest r, UserCredential c)
    {
        var e = await repository.FindAsync(x => x.Id == id && x.UserId == c.UserId, true);
        if (e is null)
        {
            notification.CreateNotification("Atualização de simulado", "Simulado não encontrado.");
            return null;
        }

        if (!await Valid(r, c)) return null;
        Map(r, e);
        e.LastUpdateDate = DateTimeOffset.UtcNow;
        if (!await repository.UpdateAsync(e))
        {
            notification.CreateNotification("Atualização de simulado", "Não foi possível atualizar o simulado.");
            return null;
        }

        await timeCapsuleCommandService.EvaluateJourneyTriggersAsync(c.UserId, r.JourneyId);
        return ToResponse(e);
    }

    public async Task<bool> DeleteAsync(long id, UserCredential c)
    {
        var e = await repository.FindAsync(x => x.Id == id && x.UserId == c.UserId, true);
        if (e is null) return notification.CreateNotification("Exclusão de simulado", "Simulado não encontrado.");
        return await repository.DeleteAsync(e) ||
               notification.CreateNotification("Exclusão de simulado", "Não foi possível excluir o simulado.");
    }

    private async Task<bool> Valid(MockAssessmentSaveRequest r, UserCredential c)
    {
        if (r.JourneyId <= 0 || string.IsNullOrWhiteSpace(r.Title) || r.TotalQuestions <= 0 || r.CorrectAnswers < 0 ||
            r.CorrectAnswers > r.TotalQuestions || r.DurationMinutes < 0)
            return notification.CreateNotification("Registro de simulado",
                "Confira o título, a quantidade de questões e os acertos.");
        var j = await journeys.FindByIdAsync(r.JourneyId, c.UserId, CancellationToken.None, true);
        if (j is null) return notification.CreateNotification("Registro de simulado", "Jornada não encontrada.");
        var ids = j.KnowledgeAreas.Select(x => x.Id).ToHashSet();
        var valid = r.Breakdown.All(x =>
                        ids.Contains(x.KnowledgeAreaId) && x.TotalQuestions >= 0 && x.CorrectAnswers >= 0 &&
                        x.VoidedQuestions >= 0 && x.CorrectAnswers + x.VoidedQuestions <= x.TotalQuestions) &&
                    r.Breakdown.Sum(x => x.TotalQuestions) <= r.TotalQuestions;
        return valid || notification.CreateNotification("Registro de simulado",
            "O detalhamento por matéria é incompatível com o total informado.");
    }

    private static void Map(MockAssessmentSaveRequest r, MockAssessment e)
    {
        e.JourneyId = r.JourneyId;
        e.Title = r.Title.Trim();
        e.Source = string.IsNullOrWhiteSpace(r.Source) ? null : r.Source.Trim();
        e.AssessmentDate = r.AssessmentDate;
        e.DurationMinutes = r.DurationMinutes;
        e.TotalQuestions = r.TotalQuestions;
        e.CorrectAnswers = r.CorrectAnswers;
        e.Score = Math.Round((decimal)r.CorrectAnswers * 100 / r.TotalQuestions, 2);
        e.Breakdown.Clear();
        foreach (var x in r.Breakdown.Where(x => x.TotalQuestions > 0))
            e.Breakdown.Add(new MockAssessmentBreakdown
            {
                KnowledgeAreaId = x.KnowledgeAreaId, TotalQuestions = x.TotalQuestions,
                CorrectAnswers = x.CorrectAnswers, VoidedQuestions = x.VoidedQuestions,
                ErrorReasonsJson = JsonSerializer.Serialize(x.ErrorReasons), Notes = x.Notes?.Trim()
            });
    }

    private static MockAssessmentResponse ToResponse(MockAssessment e) => new(e.Id, e.JourneyId, e.Title, e.Source,
        e.AssessmentDate, e.DurationMinutes, e.TotalQuestions, e.CorrectAnswers, e.Score,
        e.Breakdown.Select(x => new MockAssessmentBreakdownResponse(x.KnowledgeAreaId, x.TotalQuestions,
            x.CorrectAnswers, x.VoidedQuestions,
            JsonSerializer.Deserialize<Dictionary<string, int>>(x.ErrorReasonsJson) ?? [], x.Notes)).ToList());
}