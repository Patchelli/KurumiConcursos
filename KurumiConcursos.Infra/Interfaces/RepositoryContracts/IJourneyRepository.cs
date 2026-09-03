using KurumiConcursos.Domain.Entities;
using KurumiConcursos.Infra.Interfaces.RepositoryContracts.Base;

namespace KurumiConcursos.Infra.Interfaces.RepositoryContracts;

public interface IJourneyRepository : IReadOnlyRepository<ExamJourney>, IDisposable
{
    Task<List<ExamJourney>> FindAllByAccountAsync(Guid userId, CancellationToken cancellationToken);

    Task<ExamJourney?> FindByIdAsync(long id, Guid userId, CancellationToken cancellationToken,
        bool includeStructure = false, bool tracking = false);

    Task<KnowledgeArea?> FindAreaAsync(long id, Guid userId, CancellationToken cancellationToken,
        bool tracking = false);

    Task<SyllabusNode?> FindNodeAsync(long id, Guid userId, CancellationToken cancellationToken,
        bool tracking = false);

    Task<bool> SaveAsync(ExamJourney journey);
    Task<bool> UpdateAsync(ExamJourney journey);

    Task<bool> DeleteAsync(ExamJourney journey);
    Task<bool> SaveAreaAsync(KnowledgeArea area);
    Task<bool> UpdateAreaAsync(KnowledgeArea area);
    Task<bool> DeleteAreaAsync(KnowledgeArea area);
    Task<bool> SaveNodeAsync(SyllabusNode node);
    Task<bool> UpdateNodeAsync(SyllabusNode node);
    Task<bool> DeleteNodeAsync(SyllabusNode node);
}