using System.Linq.Expressions;
using KurumiConcursos.Domain.Entities;
using KurumiConcursos.Infra.Interfaces.RepositoryContracts;
using KurumiConcursos.Infra.ORM.Context;
using KurumiConcursos.Infra.Repositories.Base;
using Microsoft.EntityFrameworkCore;

namespace KurumiConcursos.Infra.Repositories;

public sealed class MockAssessmentRepository(ApplicationContext context)
    : RepositoryBase<MockAssessment>(context), IMockAssessmentRepository
{
    public async Task<bool> SaveAsync(MockAssessment entity)
    {
        await DbSetContext.AddAsync(entity);
        return await SaveInDatabaseAsync();
    }

    public Task<bool> UpdateAsync(MockAssessment entity)
    {
        DbSetContext.Update(entity);
        return SaveInDatabaseAsync();
    }

    public Task<bool> DeleteAsync(MockAssessment entity)
    {
        DbSetContext.Remove(entity);
        return SaveInDatabaseAsync();
    }

    public Task<MockAssessment?> FindAsync(Expression<Func<MockAssessment, bool>> predicate, bool tracking = false)
    {
        IQueryable<MockAssessment> q = DbSetContext.Include(x => x.Breakdown);
        if (!tracking) q = q.AsNoTracking();
        return q.FirstOrDefaultAsync(predicate);
    }

    public async Task<IList<MockAssessment>> FindAllAsync(Expression<Func<MockAssessment, bool>> predicate) =>
        await DbSetContext.AsNoTracking().Include(x => x.Breakdown).Where(predicate)
            .OrderByDescending(x => x.AssessmentDate).ThenByDescending(x => x.Id).ToListAsync();
}