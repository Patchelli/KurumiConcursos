using KurumiConcursos.ApplicationService.DataTransferObjects.JourneyDtos.Request;
using KurumiConcursos.ApplicationService.DataTransferObjects.JourneyDtos.Response;
using KurumiConcursos.Domain.Entities;

namespace KurumiConcursos.ApplicationService.Interfaces.MapperContracts;

public interface ISyllabusNodeStudyMapper
{
    FocusSession DtoToFocusSession(Guid userId, SyllabusNode node, SyllabusNodeStudyRequest request,
        DateOnly studyDate);

    ReviewAppointment DtoToReviewAppointment(Guid userId, SyllabusNode node, DateOnly reviewDate);

    SyllabusNodeStudyResponse DomainToDtoResponse(SyllabusNode node, int studiedMinutes, DateOnly? reviewDate);

    IList<SyllabusNodeStudyResponse> DomainToDtoResponseList(
        IEnumerable<(SyllabusNode Node, int StudiedMinutes, DateOnly? ReviewDate)> values);
}