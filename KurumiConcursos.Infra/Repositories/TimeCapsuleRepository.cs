using System.Linq.Expressions;
using KurumiConcursos.Domain.Entities;
using KurumiConcursos.Infra.Interfaces.RepositoryContracts;
using KurumiConcursos.Infra.ORM.Context;
using KurumiConcursos.Infra.Repositories.Base;
using Microsoft.EntityFrameworkCore;

namespace KurumiConcursos.Infra.Repositories;

public sealed class TimeCapsuleRepository(ApplicationContext context)
    : RepositoryBase<TimeCapsule>(context), ITimeCapsuleRepository
{
    public async Task<bool> SaveAsync(TimeCapsule entity)
    {
        await DbSetContext.AddAsync(entity);
        return await SaveInDatabaseAsync();
    }

    public Task<bool> UpdateAsync(TimeCapsule entity)
    {
        DbSetContext.Update(entity);
        return SaveInDatabaseAsync();
    }

    public Task<bool> DeleteAsync(TimeCapsule entity)
    {
        DbSetContext.Remove(entity);
        return SaveInDatabaseAsync();
    }

    public Task<TimeCapsule?> FindAsync(Expression<Func<TimeCapsule, bool>> predicate, bool tracking = false) =>
        (tracking ? DbSetContext : DbSetContext.AsNoTracking()).FirstOrDefaultAsync(predicate);

    public async Task<IList<TimeCapsule>> FindAllAsync(
        Expression<Func<TimeCapsule, bool>> predicate,
        bool tracking = false) =>
        await (tracking ? DbSetContext : DbSetContext.AsNoTracking())
            .Where(predicate)
            .OrderByDescending(item => item.CreationDate)
            .ToListAsync();
}