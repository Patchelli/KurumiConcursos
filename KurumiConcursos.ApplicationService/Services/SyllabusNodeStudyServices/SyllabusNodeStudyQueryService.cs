using KurumiConcursos.ApplicationService.DataTransferObjects.JourneyDtos.Response;
using KurumiConcursos.ApplicationService.Interfaces.MapperContracts;
using KurumiConcursos.ApplicationService.Interfaces.ServiceContracts;
using KurumiConcursos.Domain.ValueObjects;
using KurumiConcursos.Infra.Interfaces.RepositoryContracts;

namespace KurumiConcursos.ApplicationService.Services.SyllabusNodeStudyServices;

public sealed class SyllabusNodeStudyQueryService(
    IJourneyRepository journeyRepository,
    IFocusSessionRepository focusSessionRepository,
    IReviewAppointmentRepository reviewAppointmentRepository,
    ISyllabusNodeStudyMapper mapper) : ISyllabusNodeStudyQueryService
{
    public async Task<IList<SyllabusNodeStudyResponse>> FindAllAsync(long journeyId, UserCredential credential)
    {
        var journey = await journeyRepository.FindByIdAsync(journeyId, credential.UserId, CancellationToken.None,
            includeStructure: true);
        if (journey is null) return [];

        var nodes = journey.KnowledgeAreas.SelectMany(area => area.SyllabusNodes).ToList();
        var ids = nodes.Select(node => node.Id).ToHashSet();
        if (ids.Count == 0) return [];

        var sessions = await focusSessionRepository.FindAllAsync(x =>
            x.UserId == credential.UserId && x.SyllabusNodeId.HasValue && ids.Contains(x.SyllabusNodeId.Value));
        var appointments = await reviewAppointmentRepository.FindAllAsync(x =>
            x.UserId == credential.UserId && ids.Contains(x.SyllabusNodeId) && !x.Completed && !x.Superseded);
        var secondsByNode = sessions.GroupBy(x => x.SyllabusNodeId!.Value)
            .ToDictionary(group => group.Key, group => group.Sum(x => x.DurationSeconds));
        var childrenByParent = nodes.Where(x => x.ParentId.HasValue)
            .GroupBy(x => x.ParentId!.Value).ToDictionary(x => x.Key, x => x.Select(n => n.Id).ToList());
        int TotalSeconds(long nodeId) => secondsByNode.GetValueOrDefault(nodeId) +
            (childrenByParent.TryGetValue(nodeId, out var children) ? children.Sum(TotalSeconds) : 0);
        var reviewsByNode = appointments.GroupBy(x => x.SyllabusNodeId)
            .ToDictionary(group => group.Key, group => group.Min(x => x.ScheduledFor));

        return mapper.DomainToDtoResponseList(nodes.Select(node => (
            Node: node,
            StudiedMinutes: TotalSeconds(node.Id) / 60,
            ReviewDate: reviewsByNode.TryGetValue(node.Id, out var reviewDate)
                ? (DateOnly?)reviewDate
                : null)));
    }
}
