using System.Diagnostics;
using Microsoft.AspNetCore.Mvc.Filters;

namespace KurumiConcursos.Api.Filters;

// TEMP-PERF-JOURNEY: development-only diagnostic. Remove after the get_by_id investigation.
public sealed class TemporaryJourneyPerformanceResultFilter(
    ILogger<TemporaryJourneyPerformanceResultFilter> logger) : IAsyncResultFilter
{
    public async Task OnResultExecutionAsync(ResultExecutingContext context, ResultExecutionDelegate next)
    {
        if (!context.HttpContext.Request.Path.Equals("/api/journeys/get_by_id",
                StringComparison.OrdinalIgnoreCase))
        {
            await next();
            return;
        }

        var stopwatch = Stopwatch.StartNew();
        await next();
        stopwatch.Stop();
        logger.LogWarning(
            "[TEMP-PERF-JOURNEY] result_serialization elapsed_ms={ElapsedMs} trace_id={TraceId} content_length={ContentLength}",
            stopwatch.Elapsed.TotalMilliseconds, context.HttpContext.TraceIdentifier,
            context.HttpContext.Response.ContentLength);
    }
}
