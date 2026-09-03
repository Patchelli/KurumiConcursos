using KurumiConcursos.ApplicationService.DataTransferObjects.FlashcardDtos.Response;
using KurumiConcursos.ApplicationService.Interfaces.MapperContracts;
using KurumiConcursos.ApplicationService.Interfaces.ServiceContracts;
using KurumiConcursos.Domain.Enums;
using KurumiConcursos.Domain.ValueObjects;
using KurumiConcursos.Infra.Interfaces.RepositoryContracts;

namespace KurumiConcursos.ApplicationService.Services.FlashcardServices;

public sealed class FlashcardQueryService(
    IFlashcardRepository flashcardRepository,
    IJourneyRepository journeyRepository,
    IFlashcardMapper mapper) : IFlashcardQueryService
{
    public async Task<IList<FlashcardResponse>> FindAllAsync(
        long journeyId, long? knowledgeAreaId, long? syllabusNodeId, UserCredential credential)
    {
        IReadOnlyCollection<long>? nodeIds = syllabusNodeId.HasValue ? [syllabusNodeId.Value] : null;
        var cards = await flashcardRepository.FindCardsAsync(
            credential.UserId, journeyId, knowledgeAreaId, nodeIds);
        return cards.OrderByDescending(card => card.Id)
            .Select(card => mapper.DomainToDtoResponse(card, card.Collection)).ToList();
    }

    public async Task<FlashcardPracticeResponse> FindPracticeAsync(
        long journeyId, long? knowledgeAreaId, long? syllabusNodeId,
        bool includeDescendants, UserCredential credential)
    {
        IReadOnlyCollection<long>? nodeIds = null;
        if (syllabusNodeId.HasValue)
        {
            var journey = await journeyRepository.FindByIdAsync(
                journeyId, credential.UserId, CancellationToken.None, includeStructure: true);
            var nodes = journey?.KnowledgeAreas.SelectMany(area => area.SyllabusNodes).ToList() ?? [];
            if (!nodes.Any(node => node.Id == syllabusNodeId.Value))
                return new FlashcardPracticeResponse(0, 0, 0, 0, []);
            var ids = new HashSet<long> { syllabusNodeId.Value };
            if (includeDescendants)
            {
                var queue = new Queue<long>([syllabusNodeId.Value]);
                while (queue.Count > 0)
                {
                    var parentId = queue.Dequeue();
                    foreach (var child in nodes.Where(node => node.ParentId == parentId))
                        if (ids.Add(child.Id))
                            queue.Enqueue(child.Id);
                }
            }

            nodeIds = ids;
        }

        var cards = await flashcardRepository.FindCardsAsync(
            credential.UserId, journeyId, knowledgeAreaId, nodeIds);
        var today = CurrentDate();
        var eligible = cards
            .Where(card => !card.NextReviewOn.HasValue || card.NextReviewOn <= today)
            .OrderBy(card => card.NextReviewOn.HasValue ? 0 : 1)
            .ThenBy(card => card.NextReviewOn)
            .ThenBy(card => card.Id)
            .Take(50)
            .Select(card => mapper.DomainToDtoResponse(card, card.Collection))
            .ToList();
        var correctToday = cards.SelectMany(card => card.Recalls).Count(recall =>
            (recall.Grade == ERecallGrade.Good || recall.Grade == ERecallGrade.Easy) &&
            DateOnly.FromDateTime(TimeZoneInfo.ConvertTime(recall.AnsweredAt, TimeZone()).DateTime) == today);

        return new FlashcardPracticeResponse(
            cards.Count,
            cards.Count(card => card.NextReviewOn.HasValue && card.NextReviewOn <= today),
            cards.Count(card => !card.NextReviewOn.HasValue),
            correctToday,
            eligible);
    }

    private static TimeZoneInfo TimeZone() => TimeZoneInfo.FindSystemTimeZoneById(
        OperatingSystem.IsWindows() ? "E. South America Standard Time" : "America/Sao_Paulo");

    private static DateOnly CurrentDate() =>
        DateOnly.FromDateTime(TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow, TimeZone()));
}