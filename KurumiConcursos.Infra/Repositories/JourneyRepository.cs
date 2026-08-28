using System.Data;
using System.Diagnostics;
using Microsoft.EntityFrameworkCore;
using KurumiConcursos.Domain.Entities;
using KurumiConcursos.Infra.Diagnostics;
using KurumiConcursos.Infra.Interfaces.RepositoryContracts;
using KurumiConcursos.Infra.ORM.Context;
using KurumiConcursos.Infra.Repositories.Base;

namespace KurumiConcursos.Infra.Repositories;

public sealed class JourneyRepository(
    ApplicationContext dbContext,
    TemporaryJourneyPerformanceProbe performanceProbe)
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
        await using var transaction = await Context.Database.BeginTransactionAsync();

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

        var saved = await Context.SaveChangesAsync() > 0;
        await transaction.CommitAsync();
        return saved;
    }

    public async Task<bool> DeleteAsync(ExamJourney journey)
    {
        RemoveEntity(journey);
        return await SaveInDatabaseAsync();
    }

    public Task<List<ExamJourney>> FindAllByAccountAsync(Guid accountId, CancellationToken ct) => DbSetContext
        .AsNoTracking().Where(x => x.AccountId == accountId).Include(x => x.KnowledgeAreas)
        .OrderByDescending(x => x.CreationDate).ToListAsync(ct);

    public async Task<ExamJourney?> FindByIdAsync(long id, Guid accountId, CancellationToken ct,
        bool includeStructure = false, bool tracking = false)
    {
        IQueryable<ExamJourney> q = tracking ? DbSetContext : DbSetContext.AsNoTracking();
        if (includeStructure)
            q = q.Include(x => x.KnowledgeAreas)
                .ThenInclude(x => x.SyllabusNodes)
                .AsSplitQuery();

        // TEMP-PERF-JOURNEY: isolate connection, LINQ compilation and execution/materialization.
        if (Context.Database.GetDbConnection().State != ConnectionState.Open)
        {
            var connectionStopwatch = Stopwatch.StartNew();
            await Context.Database.OpenConnectionAsync(ct);
            connectionStopwatch.Stop();
            performanceProbe.ConnectionOpenMs = connectionStopwatch.Elapsed.TotalMilliseconds;
        }

        var filteredQuery = q.Where(x => x.Id == id && x.AccountId == accountId);
        var compilationStopwatch = Stopwatch.StartNew();
        _ = filteredQuery.ToQueryString();
        compilationStopwatch.Stop();
        performanceProbe.QueryCompilationMs = compilationStopwatch.Elapsed.TotalMilliseconds;

        var executionStopwatch = Stopwatch.StartNew();
        var journey = await filteredQuery.FirstOrDefaultAsync(ct);
        executionStopwatch.Stop();
        performanceProbe.QueryExecutionMs = executionStopwatch.Elapsed.TotalMilliseconds;
        return journey;
    }

    public Task<KnowledgeArea?> FindAreaAsync(long id, Guid accountId, CancellationToken ct, bool tracking = false)
    {
        IQueryable<KnowledgeArea> q = Context.Set<KnowledgeArea>();
        if (!tracking) q = q.AsNoTracking();
        return q.Include(x => x.SyllabusNodes)
            .FirstOrDefaultAsync(x => x.Id == id && x.Journey.AccountId == accountId, ct);
    }

    public Task<SyllabusNode?> FindNodeAsync(long id, Guid accountId, CancellationToken ct, bool tracking = false)
    {
        IQueryable<SyllabusNode> q = Context.Set<SyllabusNode>();
        if (!tracking) q = q.AsNoTracking();
        return q.FirstOrDefaultAsync(x => x.Id == id && x.KnowledgeArea.Journey.AccountId == accountId, ct);
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

    public async Task<bool> DeleteNodeAsync(SyllabusNode node)
    {
        await Context.Set<SyllabusNode>().Where(item => item.ParentId == node.Id).ExecuteDeleteAsync();
        Context.Set<SyllabusNode>().Remove(node);
        return await Context.SaveChangesAsync() > 0;
    }

}
