namespace KurumiConcursos.Infra.Diagnostics;

// TEMP-PERF-JOURNEY: scoped diagnostic data shared with the application service.
public sealed class TemporaryJourneyPerformanceProbe
{
    public double ConnectionOpenMs { get; set; }
    public double QueryCompilationMs { get; set; }
    public double QueryExecutionMs { get; set; }
}
