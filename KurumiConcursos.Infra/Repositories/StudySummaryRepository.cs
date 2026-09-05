using System.Linq.Expressions;
using KurumiConcursos.Domain.Entities;
using KurumiConcursos.Infra.Interfaces.RepositoryContracts;
using KurumiConcursos.Infra.ORM.Context;
using KurumiConcursos.Infra.Repositories.Base;
using Microsoft.EntityFrameworkCore;

namespace KurumiConcursos.Infra.Repositories;

public sealed class StudySummaryRepository(ApplicationContext context)
    : RepositoryBase<StudySummary>(context), IStudySummaryRepository
{
    public async Task<bool> SaveAsync(StudySummary e)
    {
        await DbSetContext.AddAsync(e);
        return await SaveInDatabaseAsync();
    }

    public async Task<IList<StudySummary>> FindAllAsync(Expression<Func<StudySummary, bool>> p) => await DbSetContext
        .AsNoTracking().Where(p).OrderByDescending(x => x.CreationDate).ThenByDescending(x => x.Id).ToListAsync();
}