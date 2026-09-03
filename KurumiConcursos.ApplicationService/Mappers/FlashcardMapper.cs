using KurumiConcursos.ApplicationService.DataTransferObjects.FlashcardDtos.Request;
using KurumiConcursos.ApplicationService.DataTransferObjects.FlashcardDtos.Response;
using KurumiConcursos.ApplicationService.Interfaces.MapperContracts;
using KurumiConcursos.Domain.Entities;

namespace KurumiConcursos.ApplicationService.Mappers;

public sealed class FlashcardMapper : IFlashcardMapper
{
    public MemoryCard DtoRegisterToDomain(long collectionId, FlashcardRegisterRequest request) => new()
    {
        FlashCollectionId = collectionId,
        Front = request.Front.Trim(),
        Back = request.Back.Trim(),
        Model = request.Model.Trim(),
        Type = request.Type.Trim(),
        CorrectAnswer = request.CorrectAnswer
    };

    public FlashcardResponse DomainToDtoResponse(MemoryCard card, FlashCollection collection) => new(
        card.Id, collection.JourneyId, collection.KnowledgeAreaId!.Value,
        collection.SyllabusNodeId, card.Model, card.Type, card.Front, card.Back,
        card.CorrectAnswer, card.NextReviewOn);
}