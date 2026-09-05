using KurumiConcursos.Domain.Entities;

namespace KurumiConcursos.Infra.Interfaces.RepositoryContracts;

public interface IStudyTimerSessionRepository
{
    Task<StudyTimerSession?> FindByUserAsync(Guid userId, bool tracking = false);
    Task<bool> SaveAsync(StudyTimerSession session);
    Task<bool> UpdateAsync(StudyTimerSession session);
    Task<bool> DeleteAsync(StudyTimerSession session);
}
