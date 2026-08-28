using System.Diagnostics;
using KurumiConcursos.ApplicationService.DataTransferObjects.JourneyDtos.Response;
using KurumiConcursos.ApplicationService.Interfaces.MapperContracts;
using KurumiConcursos.ApplicationService.Interfaces.ServiceContracts;
using KurumiConcursos.Infra.Interfaces.RepositoryContracts;
using KurumiConcursos.Infra.Diagnostics;
using KurumiConcursos.Domain.ValueObjects;
using Microsoft.Extensions.Logging;

namespace KurumiConcursos.ApplicationService.Services.JourneyServices;

public sealed class JourneyQueryService(
    IJourneyRepository repository,
    IJourneyMapper mapper,
    TemporaryJourneyPerformanceProbe performanceProbe,
    ILogger<JourneyQueryService> logger) : IJourneyQueryService
{
    public async Task<IList<JourneySummaryResponse>> FindAllAsync(UserCredential userCredential) =>
        mapper.DomainToDtoSummaryResponseList(
            await repository.FindAllByAccountAsync(userCredential.UserId, CancellationToken.None));

    public async Task<JourneyDetailsResponse?> FindByIdAsync(long id, UserCredential userCredential)
    {
        // TEMP-PERF-JOURNEY: remove after the get_by_id investigation.
        var totalStopwatch = Stopwatch.StartNew();
        var repositoryStopwatch = Stopwatch.StartNew();
        var journey = await repository.FindByIdAsync(id, userCredential.UserId, CancellationToken.None, true);
        repositoryStopwatch.Stop();
        if (journey is null)
        {
            logger.LogWarning(
                "[TEMP-PERF-JOURNEY] service_complete elapsed_ms={ElapsedMs} repository_ms={RepositoryMs} journey_id={JourneyId} found=false",
                totalStopwatch.Elapsed.TotalMilliseconds, repositoryStopwatch.Elapsed.TotalMilliseconds, id);
            return null;
        }

        var mapperStopwatch = Stopwatch.StartNew();
        var response = mapper.DomainToDtoDetailsResponse(journey);
        mapperStopwatch.Stop();
        totalStopwatch.Stop();
        logger.LogWarning(
            "[TEMP-PERF-JOURNEY] service_complete elapsed_ms={ElapsedMs} repository_ms={RepositoryMs} connection_open_ms={ConnectionOpenMs} query_compile_ms={QueryCompileMs} query_execute_ms={QueryExecuteMs} mapper_ms={MapperMs} journey_id={JourneyId} areas={AreaCount} nodes={NodeCount}",
            totalStopwatch.Elapsed.TotalMilliseconds, repositoryStopwatch.Elapsed.TotalMilliseconds,
            performanceProbe.ConnectionOpenMs, performanceProbe.QueryCompilationMs,
            performanceProbe.QueryExecutionMs, mapperStopwatch.Elapsed.TotalMilliseconds, id, journey.KnowledgeAreas.Count,
            journey.KnowledgeAreas.Sum(area => area.SyllabusNodes.Count));
        return response;
    }
}
