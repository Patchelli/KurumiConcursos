using KurumiConcursos.Domain.Entities;

namespace KurumiConcursos.Infra.Interfaces.RepositoryContracts;

public interface ITopicMaterialRepository
{
    Task<IList<TopicMaterial>> FindAllAsync(long syllabusNodeId, Guid userId);
    Task<TopicMaterial?> FindAsync(long id, long syllabusNodeId, Guid userId, bool tracking = false);
    Task<IList<TopicMaterial>> FindAllByAreaAsync(long knowledgeAreaId, Guid userId);
    Task<TopicMaterial?> FindByAreaAsync(long id, long knowledgeAreaId, Guid userId, bool tracking = false);
    Task<bool> SaveAsync(TopicMaterial material);
    Task<bool> DeleteAsync(TopicMaterial material);
}
