namespace KurumiConcursos.ApplicationService.Services.PrivateMaterialServices;

public sealed record PrivateMaterialFile(string Name, Stream Stream, IDisposable Disposable);