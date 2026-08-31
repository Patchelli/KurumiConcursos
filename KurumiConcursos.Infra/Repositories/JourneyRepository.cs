using KurumiConcursos.Domain.Entities;
using KurumiConcursos.Infra.Interfaces.RepositoryContracts;
using KurumiConcursos.Infra.ORM.Context;
using KurumiConcursos.Infra.Repositories.Base;
using Microsoft.EntityFrameworkCore;

namespace KurumiConcursos.Infra.Repositories;

public sealed class JourneyRepository(ApplicationContext dbContext)
    : RepositoryBase<ExamJourney>(dbContext), IJourneyRepository
{
    public async Task<bool> SaveAsync(ExamJourney journey)
    {
        AddEntity(journey);
        return await SaveInDatabaseAsync();
    }

    public async Task<bool> UpdateAsync(ExamJourney journey)
    {
        var areas = journey.KnowledgeAreas.ToList();
        Context.ChangeTracker.Clear();
        await Context.Set<SyllabusNode>()
            .Where(node => node.KnowledgeArea.JourneyId == journey.Id)
            .ExecuteDeleteAsync();
        await Context.Set<KnowledgeArea>()
            .Where(area => area.JourneyId == journey.Id)
            .ExecuteDeleteAsync();

        journey.KnowledgeAreas = [];
        Context.Attach(journey);
        Context.Entry(journey).State = EntityState.Modified;
        Context.Set<KnowledgeArea>().AddRange(areas);

        return await Context.SaveChangesAsync() > 0;
    }

    public async Task<bool> DeleteAsync(ExamJourney journey)
    {
        RemoveEntity(journey);
        return await SaveInDatabaseAsync();
    }

    public Task<List<ExamJourney>> FindAllByAccountAsync(Guid userId, CancellationToken ct) => DbSetContext
        .AsNoTracking().Where(x => x.UserId == userId).Include(x => x.KnowledgeAreas)
        .OrderByDescending(x => x.CreationDate).ToListAsync(ct);

    public async Task<ExamJourney?> FindByIdAsync(long id, Guid userId, CancellationToken ct,
        bool includeStructure = false, bool tracking = false)
    {
        IQueryable<ExamJourney> q = tracking ? DbSetContext : DbSetContext.AsNoTracking();
        if (includeStructure)
            q = q.Include(x => x.KnowledgeAreas)
                .ThenInclude(x => x.SyllabusNodes)
                .AsSplitQuery();

        return await q.FirstOrDefaultAsync(x => x.Id == id && x.UserId == userId, ct);
    }

    public Task<KnowledgeArea?> FindAreaAsync(long id, Guid userId, CancellationToken ct, bool tracking = false)
    {
        IQueryable<KnowledgeArea> q = Context.Set<KnowledgeArea>();
        if (!tracking) q = q.AsNoTracking();
        return q.Include(x => x.SyllabusNodes)
            .FirstOrDefaultAsync(x => x.Id == id && x.Journey.UserId == userId, ct);
    }

    public Task<SyllabusNode?> FindNodeAsync(long id, Guid userId, CancellationToken ct, bool tracking = false)
    {
        IQueryable<SyllabusNode> q = Context.Set<SyllabusNode>();
        if (!tracking) q = q.AsNoTracking();
        return q.FirstOrDefaultAsync(x => x.Id == id && x.KnowledgeArea.Journey.UserId == userId, ct);
    }

    public async Task<bool> SaveAreaAsync(KnowledgeArea area)
    {
        Context.Set<KnowledgeArea>().Add(area);
        return await Context.SaveChangesAsync() > 0;
    }

    public async Task<bool> DeleteAreaAsync(KnowledgeArea area)
    {
        Context.Set<KnowledgeArea>().Remove(area);
        return await Context.SaveChangesAsync() > 0;
    }

    public async Task<bool> SaveNodeAsync(SyllabusNode node)
    {
        Context.Set<SyllabusNode>().Add(node);
        return await Context.SaveChangesAsync() > 0;
    }

    public async Task<bool> UpdateNodeAsync(SyllabusNode node)
    {
        if (Context.Entry(node).State == EntityState.Detached)
            Context.Attach(node);
        Context.Entry(node).State = EntityState.Modified;
        return await Context.SaveChangesAsync() > 0;
    }

    public async Task<bool> DeleteNodeAsync(SyllabusNode node)
    {
        await Context.Set<SyllabusNode>().Where(item => item.ParentId == node.Id).ExecuteDeleteAsync();
        Context.Set<SyllabusNode>().Remove(node);
        return await Context.SaveChangesAsync() > 0;
    }
}