using KurumiConcursos.ApplicationService.DataTransferObjects.JourneyDtos.Request;
using KurumiConcursos.ApplicationService.DataTransferObjects.JourneyDtos.Response;
using KurumiConcursos.ApplicationService.Interfaces.MapperContracts;
using KurumiConcursos.Domain.Entities;

namespace KurumiConcursos.ApplicationService.Mappers;

public sealed class SyllabusNodeStudyMapper : ISyllabusNodeStudyMapper
{
    public FocusSession DtoToFocusSession(Guid userId, SyllabusNode node, SyllabusNodeStudyRequest request,
        DateOnly studyDate) => new()
    {
        UserId = userId,
        JourneyId = request.JourneyId,
        KnowledgeAreaId = node.KnowledgeAreaId,
        SyllabusNodeId = node.Id,
        StudyDate = studyDate,
        DurationSeconds = Math.Max(0, request.StudiedSeconds ?? request.StudiedMinutes * 60)
    };

    public ReviewAppointment DtoToReviewAppointment(Guid userId, SyllabusNode node, DateOnly reviewDate) => new()
    {
        UserId = userId,
        SyllabusNodeId = node.Id,
        ScheduledFor = reviewDate,
        Completed = false,
        Superseded = false
    };

    public SyllabusNodeStudyResponse DomainToDtoResponse(SyllabusNode node, int studiedMinutes, DateOnly? reviewDate) =>
        new(node.Id, node.Progress, node.StudyStartedOn, node.StudiedOn, studiedMinutes, reviewDate);

    public IList<SyllabusNodeStudyResponse> DomainToDtoResponseList(
        IEnumerable<(SyllabusNode Node, int StudiedMinutes, DateOnly? ReviewDate)> values) =>
        values.Select(value => DomainToDtoResponse(value.Node, value.StudiedMinutes, value.ReviewDate)).ToList();
}
