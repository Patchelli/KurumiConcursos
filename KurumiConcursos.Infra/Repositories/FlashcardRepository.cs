using KurumiConcursos.Domain.Entities;
using KurumiConcursos.Infra.Interfaces.RepositoryContracts;
using KurumiConcursos.Infra.ORM.Context;
using Microsoft.EntityFrameworkCore;

namespace KurumiConcursos.Infra.Repositories;

public sealed class FlashcardRepository(ApplicationContext context) : IFlashcardRepository
{
    public Task<FlashCollection?> FindCollectionAsync(Guid userId, long journeyId, long knowledgeAreaId,
        long? syllabusNodeId) =>
        context.Set<FlashCollection>().FirstOrDefaultAsync(item =>
            item.UserId == userId && item.JourneyId == journeyId &&
            item.KnowledgeAreaId == knowledgeAreaId && item.SyllabusNodeId == syllabusNodeId);

    public async Task<bool> SaveCollectionAsync(FlashCollection collection)
    {
        context.Add(collection);
        return await context.SaveChangesAsync() > 0;
    }

    public async Task<bool> SaveCardAsync(MemoryCard card)
    {
        context.Add(card);
        return await context.SaveChangesAsync() > 0;
    }

    public async Task<IList<MemoryCard>> FindCardsAsync(Guid userId, long journeyId, long? knowledgeAreaId,
        IReadOnlyCollection<long>? nodeIds)
    {
        var query = context.Set<MemoryCard>().AsNoTracking()
            .Include(item => item.Collection).Include(item => item.Recalls)
            .Where(item => item.Collection.UserId == userId && item.Collection.JourneyId == journeyId);
        if (knowledgeAreaId.HasValue)
            query = query.Where(item => item.Collection.KnowledgeAreaId == knowledgeAreaId.Value);
        if (nodeIds is not null)
            query = query.Where(item =>
                item.Collection.SyllabusNodeId.HasValue && nodeIds.Contains(item.Collection.SyllabusNodeId.Value));
        return await query.ToListAsync();
    }

    public Task<MemoryCard?> FindCardAsync(long cardId, Guid userId) =>
        context.Set<MemoryCard>().Include(item => item.Collection)
            .FirstOrDefaultAsync(item => item.Id == cardId && item.Collection.UserId == userId);

    public async Task<bool> SaveRecallAsync(MemoryCard card, MemoryRecall recall)
    {
        context.Update(card);
        context.Add(recall);
        return await context.SaveChangesAsync() > 0;
    }

    public async Task<bool> UpdateCardAsync(MemoryCard card)
    {
        context.Update(card);
        return await context.SaveChangesAsync() > 0;
    }

    public async Task<bool> DeleteCardAsync(MemoryCard card)
    {
        context.Remove(card);
        return await context.SaveChangesAsync() > 0;
    }
}