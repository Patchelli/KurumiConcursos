using KurumiConcursos.Domain.Entities;

namespace KurumiConcursos.Infra.Interfaces.RepositoryContracts;

public interface IFlashcardRepository
{
    Task<FlashCollection?> FindCollectionAsync(Guid userId, long journeyId, long knowledgeAreaId, long? syllabusNodeId);
    Task<bool> SaveCollectionAsync(FlashCollection collection);
    Task<bool> SaveCardAsync(MemoryCard card);

    Task<IList<MemoryCard>> FindCardsAsync(Guid userId, long journeyId, long? knowledgeAreaId,
        IReadOnlyCollection<long>? nodeIds);

    Task<MemoryCard?> FindCardAsync(long cardId, Guid userId);
    Task<bool> SaveRecallAsync(MemoryCard card, MemoryRecall recall);
    Task<bool> UpdateCardAsync(MemoryCard card);
    Task<bool> DeleteCardAsync(MemoryCard card);
}