using KurumiConcursos.Domain.Enums;

namespace KurumiConcursos.ApplicationService.DataTransferObjects.JourneyDtos.Request;

public sealed record UpdateNodeProgressRequest(EStudyProgress Progress, DateOnly Date);