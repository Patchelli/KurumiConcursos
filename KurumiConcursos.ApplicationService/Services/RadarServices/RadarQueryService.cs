using System.Text.Json;
using KurumiConcursos.ApplicationService.DataTransferObjects.RadarDtos.Response;
using KurumiConcursos.ApplicationService.Interfaces.MapperContracts;
using KurumiConcursos.ApplicationService.Interfaces.ServiceContracts;
using KurumiConcursos.ApplicationService.Traces;
using KurumiConcursos.Domain.Interface;
using KurumiConcursos.Domain.ValueObjects;
using KurumiConcursos.Infra.Interfaces.RepositoryContracts;
using KurumiConcursos.Infra.Interfaces.ServiceContracts;
using Microsoft.Extensions.Logging;

namespace KurumiConcursos.ApplicationService.Services.RadarServices;

public sealed class RadarQueryService(
    IUserRepository users,
    IRadarMapper mapper,
    IPciContestQueryService pci,
    INotificationHandler notification,
    ILogger<RadarQueryService> logger) : IRadarQueryService
{
    public async Task<RadarPreferencesResponse?> GetPreferencesAsync(UserCredential credential)
    {
        var user = await users.FindByPredicateAsync(item => item.Id == credential.UserId, toQuery: true);
        if (user is not null) return mapper.DomainToPreferencesResponse(user);
        notification.CreateNotification(RadarTrace.Query, "Usuário não encontrado.");
        return null;
    }

    public async Task<RadarResultResponse?> GetContestsAsync(UserCredential credential,
        CancellationToken cancellationToken)
    {
        var preferences = await GetPreferencesAsync(credential);
        if (preferences is null) return null;
        try
        {
            var feed = await pci.GetContestsAsync(cancellationToken);
            var contests = RadarContestFilter.Filter(feed.Contests, preferences);
            return mapper.DomainToResultResponse(feed with { Contests = contests });
        }
        catch (Exception ex) when (ex is HttpRequestException or JsonException ||
                                   ex is OperationCanceledException && !cancellationToken.IsCancellationRequested)
        {
            logger.LogWarning(ex, "PCI Radar unavailable");
            notification.CreateNotification(RadarTrace.Query,
                "O PCI Concursos está indisponível no momento. Tente novamente em instantes.");
            return null;
        }
    }
}