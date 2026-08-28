using KurumiConcursos.ApplicationService.DataTransferObjects.JourneyDtos.Request;
using KurumiConcursos.ApplicationService.DataTransferObjects.JourneyDtos.Response;
using KurumiConcursos.ApplicationService.Interfaces.MapperContracts;
using KurumiConcursos.Domain.Entities;

namespace KurumiConcursos.ApplicationService.Mappers;

public sealed class JourneyMapper : IJourneyMapper
{
    public ExamJourney DtoRegisterToDomain(Guid accountId, SaveJourneyStructureRequest dto)
    {
        var journey = DtoUpdateToDomain(new ExamJourney { AccountId = accountId }, dto.Journey);
        journey.KnowledgeAreas = dto.KnowledgeAreas
            .OrderBy(item => item.Order)
            .Select(item => MapArea(journey, item))
            .ToList();
        return journey;
    }

    public ExamJourney DtoUpdateToDomain(ExamJourney entity, SaveJourneyRequest dto)
    {
        entity.Title = dto.Title.Trim();
        entity.Institution = dto.Institution;
        entity.ExamBoard = dto.ExamBoard;
        entity.Position = dto.Position;
        entity.Salary = dto.Salary;
        entity.Openings = dto.Openings;
        entity.NoticeUrl = dto.NoticeUrl;
        entity.ExamDate = dto.ExamDate;
        entity.Stage = dto.Stage;
        entity.IncludeInStatistics = dto.IncludeInStatistics;
        entity.LogoUrl = dto.LogoUrl;
        entity.LastUpdateDate = DateTimeOffset.UtcNow;
        return entity;
    }

    public JourneySummaryResponse DomainToDtoSummaryResponse(ExamJourney entity) =>
        new(entity.Id, entity.Title, entity.Institution, entity.Position, entity.ExamDate,
            entity.Stage, entity.KnowledgeAreas.Count, entity.LogoUrl);

    public JourneyDetailsResponse DomainToDtoDetailsResponse(ExamJourney entity) =>
        new(entity.Id, entity.Title, entity.Institution, entity.ExamBoard, entity.Position,
            entity.Salary, entity.Openings, entity.NoticeUrl, entity.ExamDate, entity.Stage,
            entity.IncludeInStatistics, entity.LogoUrl,
            entity.KnowledgeAreas.OrderBy(item => item.Order).Select(MapAreaResponse).ToList());

    public IList<JourneySummaryResponse> DomainToDtoSummaryResponseList(IList<ExamJourney> entities) =>
        entities.Select(DomainToDtoSummaryResponse).ToList();

    private static KnowledgeArea MapArea(ExamJourney journey, SaveKnowledgeAreaStructureRequest dto)
    {
        var area = new KnowledgeArea
        {
            Journey = journey,
            Title = dto.Title.Trim().ToUpperInvariant(),
            Order = dto.Order,
            LastUpdateDate = DateTimeOffset.UtcNow
        };
        area.SyllabusNodes = dto.Nodes.OrderBy(item => item.Order)
            .Select(item => MapNode(area, null, item)).SelectMany(FlattenNode).ToList();
        return area;
    }

    private static SyllabusNode MapNode(KnowledgeArea area, SyllabusNode? parent, SaveSyllabusNodeStructureRequest dto)
    {
        var node = new SyllabusNode
        {
            KnowledgeArea = area,
            Parent = parent,
            Title = dto.Title.Trim(),
            Order = dto.Order,
            LastUpdateDate = DateTimeOffset.UtcNow
        };
        node.Children = dto.Children.OrderBy(item => item.Order)
            .Select(item => MapNode(area, node, item)).ToList();
        return node;
    }

    private static IEnumerable<SyllabusNode> FlattenNode(SyllabusNode node)
    {
        yield return node;
        foreach (var child in node.Children.SelectMany(FlattenNode))
            yield return child;
    }

    private static KnowledgeAreaResponse MapAreaResponse(KnowledgeArea entity)
    {
        var childrenByParent = entity.SyllabusNodes
            .Where(node => node.ParentId.HasValue)
            .GroupBy(node => node.ParentId!.Value)
            .ToDictionary(group => group.Key, group => group.OrderBy(node => node.Order).ToList());

        SyllabusNodeResponse MapNodeResponse(SyllabusNode node) =>
            new(node.Id, node.ParentId, node.Title, node.Order, node.Progress,
                node.StudyStartedOn, node.StudiedOn,
                childrenByParent.TryGetValue(node.Id, out var children)
                    ? children.Select(MapNodeResponse).ToList()
                    : []);

        var roots = entity.SyllabusNodes
            .Where(node => node.ParentId is null)
            .OrderBy(node => node.Order)
            .Select(MapNodeResponse)
            .ToList();

        return new KnowledgeAreaResponse(entity.Id, entity.Title, entity.Order,
            entity.Weight, entity.ExpectedQuestions, roots);
    }
}
