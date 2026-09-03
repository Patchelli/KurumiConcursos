using KurumiConcursos.Domain.Enums;

namespace KurumiConcursos.ApplicationService.DataTransferObjects.FlashcardDtos.Request;

public sealed record FlashcardRecallRequest(long CardId, ERecallGrade Grade);