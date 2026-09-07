using System.Net.Http.Json;
using KurumiConcursos.Domain.Providers;
using KurumiConcursos.Domain.ValueObjects;
using KurumiConcursos.Infra.DataTransferObjects.PciDtos.Request;
using KurumiConcursos.Infra.Interfaces.MapperContracts;
using KurumiConcursos.Infra.Interfaces.ServiceContracts;
using Microsoft.Extensions.Caching.Memory;

namespace KurumiConcursos.Infra.Services.PciContestServices;

public sealed class PciContestQueryService(
    HttpClient httpClient,
    IPciContestMapper pciContestMapper,
    IMemoryCache cache,
    PciConcursosOptions options) : IPciContestQueryService
{
    private const string CacheKey = "pci:contests";

    public async Task<ContestFeed> GetContestsAsync(CancellationToken cancellationToken)
    {
        var feed = await cache.GetOrCreateAsync(CacheKey, async entry =>
        {
            entry.AbsoluteExpirationRelativeToNow = TimeSpan.FromMinutes(options.CacheMinutes);
            using var response =
                await httpClient.PostAsJsonAsync(string.Empty, new PciToolCallRequest(), cancellationToken);
            response.EnsureSuccessStatusCode();
            var content = await response.Content.ReadAsStringAsync(cancellationToken);
            return new ContestFeed(pciContestMapper.ResponseToDomain(content), DateTimeOffset.UtcNow);
        });

        return feed!;
    }
}