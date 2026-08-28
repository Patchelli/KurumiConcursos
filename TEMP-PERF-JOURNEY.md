# TEMP-PERF-JOURNEY

Temporary diagnostics for `/api/journeys/get_by_id`. Search for `TEMP-PERF-JOURNEY` to remove every instrumentation point.

Files changed only for diagnostics:

- `KurumiConcursos.ApplicationService/Services/JourneyServices/JourneyQueryService.cs`: repository and mapper durations.
- `KurumiConcursos.Infra/Diagnostics/TemporaryJourneyPerformanceProbe.cs`: scoped timing carrier.
- `KurumiConcursos.Infra/Repositories/JourneyRepository.cs`: connection, query compilation and execution/materialization timings.
- `KurumiConcursos.Api/Filters/TemporaryJourneyPerformanceResultFilter.cs`: MVC result/JSON serialization duration.
- `KurumiConcursos.Api/Settings/Handlers/ControllersSettings.cs`: temporary filter registration.
- `KurumiConcursos.Api/IoC/InversionOfControlHandler.cs`: temporary probe registration.

Removal procedure:

1. Remove every block/comment containing `TEMP-PERF-JOURNEY` from the repository and service.
2. Delete `TemporaryJourneyPerformanceProbe.cs` and remove its IoC registration.
3. Delete `TemporaryJourneyPerformanceResultFilter.cs`.
4. Remove its namespace import, service registration, and `AddService` call from `ControllersSettings.cs`.
5. Delete this file.
6. Verify with `rg -n "TEMP-PERF-JOURNEY" KurumiConcursos.Backend`.
