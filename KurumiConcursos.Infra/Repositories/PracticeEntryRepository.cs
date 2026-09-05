using System.Linq.Expressions;
using KurumiConcursos.Domain.Entities;
using KurumiConcursos.Infra.Interfaces.RepositoryContracts;
using KurumiConcursos.Infra.ORM.Context;
using KurumiConcursos.Infra.Repositories.Base;
using Microsoft.EntityFrameworkCore;

namespace KurumiConcursos.Infra.Repositories;

public sealed class PracticeEntryRepository(ApplicationContext c)
    : RepositoryBase<PracticeEntry>(c), IPracticeEntryRepository
{
    public async Task<bool> SaveAsync(PracticeEntry e)
    {
        await DbSetContext.AddAsync(e);
        return await SaveInDatabaseAsync();
    }

    public Task<bool> UpdateAsync(PracticeEntry e)
    {
        DbSetContext.Update(e);
        return SaveInDatabaseAsync();
    }

    public Task<bool> DeleteAsync(PracticeEntry e)
    {
        DbSetContext.Remove(e);
        return SaveInDatabaseAsync();
    }

    public Task<PracticeEntry?> FindAsync(Expression<Func<PracticeEntry, bool>> p, bool tracking = false)
    {
        IQueryable<PracticeEntry> q = DbSetContext;
        if (!tracking) q = q.AsNoTracking();
        return q.FirstOrDefaultAsync(p);
    }

    public async Task<IList<PracticeEntry>> FindAllAsync(Expression<Func<PracticeEntry, bool>> p) => await DbSetContext
        .AsNoTracking().Where(p).OrderByDescending(x => x.PracticeDate).ThenByDescending(x => x.Id).ToListAsync();
}