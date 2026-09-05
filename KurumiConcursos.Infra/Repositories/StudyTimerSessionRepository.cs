using KurumiConcursos.Domain.Entities;
using KurumiConcursos.Infra.Interfaces.RepositoryContracts;
using KurumiConcursos.Infra.ORM.Context;
using KurumiConcursos.Infra.Repositories.Base;
using Microsoft.EntityFrameworkCore;

namespace KurumiConcursos.Infra.Repositories;

public sealed class StudyTimerSessionRepository(ApplicationContext dbContext)
    : RepositoryBase<StudyTimerSession>(dbContext), IStudyTimerSessionRepository
{
    public Task<StudyTimerSession?> FindByUserAsync(Guid userId, bool tracking = false)
    {
        var query = tracking ? DbSetContext : DbSetContext.AsNoTracking();
        return query.SingleOrDefaultAsync(x => x.UserId == userId);
    }

    public async Task<bool> SaveAsync(StudyTimerSession session)
    {
        await DbSetContext.AddAsync(session);
        return await SaveInDatabaseAsync();
    }

    public Task<bool> UpdateAsync(StudyTimerSession session)
    {
        DetachedObject(session);
        DbSetContext.Update(session);
        return SaveInDatabaseAsync();
    }

    public Task<bool> DeleteAsync(StudyTimerSession session)
    {
        DbSetContext.Remove(session);
        return SaveInDatabaseAsync();
    }
}
