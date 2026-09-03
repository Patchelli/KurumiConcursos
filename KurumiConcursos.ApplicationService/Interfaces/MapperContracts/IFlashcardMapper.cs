using KurumiConcursos.ApplicationService.DataTransferObjects.FlashcardDtos.Request;
using KurumiConcursos.ApplicationService.DataTransferObjects.FlashcardDtos.Response;
using KurumiConcursos.Domain.Entities;

namespace KurumiConcursos.ApplicationService.Interfaces.MapperContracts;

public interface IFlashcardMapper
{
    MemoryCard DtoRegisterToDomain(long collectionId, FlashcardRegisterRequest request);
    FlashcardResponse DomainToDtoResponse(MemoryCard card, FlashCollection collection);
}