using KurumiConcursos.Domain.Entities;
using KurumiConcursos.Infra.Interfaces.RepositoryContracts;
using KurumiConcursos.Infra.ORM.Context;
using Microsoft.EntityFrameworkCore;

namespace KurumiConcursos.Infra.Repositories;

public sealed class TopicMaterialRepository(ApplicationContext context) : ITopicMaterialRepository
{
    private IQueryable<TopicMaterial> OwnedBy(Guid userId) => context.Set<TopicMaterial>()
        .Where(item => (item.SyllabusNode != null && item.SyllabusNode.KnowledgeArea.Journey.UserId == userId) ||
                       (item.KnowledgeArea != null && item.KnowledgeArea.Journey.UserId == userId));

    public async Task<IList<TopicMaterial>> FindAllAsync(long syllabusNodeId, Guid userId) =>
        await OwnedBy(userId).AsNoTracking().Where(item => item.SyllabusNodeId == syllabusNodeId)
            .OrderBy(item => item.Name).ToListAsync();

    public async Task<IList<TopicMaterial>> FindAllByAreaAsync(long knowledgeAreaId, Guid userId) =>
        await OwnedBy(userId).AsNoTracking().Where(item => item.KnowledgeAreaId == knowledgeAreaId)
            .OrderBy(item => item.Name).ToListAsync();

    public Task<TopicMaterial?> FindAsync(long id, long syllabusNodeId, Guid userId, bool tracking = false)
    {
        var query = OwnedBy(userId);
        if (!tracking) query = query.AsNoTracking();
        return query.FirstOrDefaultAsync(item => item.Id == id && item.SyllabusNodeId == syllabusNodeId);
    }

    public Task<TopicMaterial?> FindByAreaAsync(long id, long knowledgeAreaId, Guid userId, bool tracking = false)
    {
        var query = OwnedBy(userId);
        if (!tracking) query = query.AsNoTracking();
        return query.FirstOrDefaultAsync(item => item.Id == id && item.KnowledgeAreaId == knowledgeAreaId);
    }

    public async Task<bool> SaveAsync(TopicMaterial material)
    {
        context.Add(material);
        return await context.SaveChangesAsync() > 0;
    }

    public async Task<bool> DeleteAsync(TopicMaterial material)
    {
        context.Remove(material);
        return await context.SaveChangesAsync() > 0;
    }
}
