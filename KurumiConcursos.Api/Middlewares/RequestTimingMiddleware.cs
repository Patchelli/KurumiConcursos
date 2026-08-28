using System.Diagnostics;

namespace KurumiConcursos.Api.Middlewares;

public sealed class RequestTimingMiddleware(RequestDelegate next, ILogger<RequestTimingMiddleware> logger)
{
    public async Task InvokeAsync(HttpContext context)
    {
        var stopwatch = Stopwatch.StartNew();
        await next(context);
        stopwatch.Stop();
        logger.LogWarning("TIMING [{Method}] {Path} → {Ms}ms",
            context.Request.Method, context.Request.Path, stopwatch.ElapsedMilliseconds);
    }
}
