using KurumiConcursos.ApplicationService.DataTransferObjects.FlashcardDtos.Request;
using KurumiConcursos.ApplicationService.DataTransferObjects.FlashcardDtos.Response;
using KurumiConcursos.ApplicationService.Interfaces.MapperContracts;
using KurumiConcursos.ApplicationService.Interfaces.ServiceContracts;
using KurumiConcursos.ApplicationService.Traces;
using KurumiConcursos.Domain.Entities;
using KurumiConcursos.Domain.Enums;
using KurumiConcursos.Domain.Interface;
using KurumiConcursos.Domain.ValueObjects;
using KurumiConcursos.Infra.Interfaces.RepositoryContracts;

namespace KurumiConcursos.ApplicationService.Services.FlashcardServices;

public sealed class FlashcardCommandService(
    IFlashcardRepository flashcardRepository,
    IJourneyRepository journeyRepository,
    IFlashcardMapper mapper,
    IValidate<MemoryCard> validation,
    INotificationHandler notification,
    ILoggerHandler logger)
    : ServiceBase<MemoryCard>(notification, validation, logger), IFlashcardCommandService
{
    public async Task<FlashcardResponse?> RegisterAsync(FlashcardRegisterRequest request, UserCredential credential)
    {
        var journey = await journeyRepository.FindByIdAsync(
            request.JourneyId, credential.UserId, CancellationToken.None, includeStructure: true);
        var area = journey?.KnowledgeAreas.FirstOrDefault(item => item.Id == request.KnowledgeAreaId);
        var node = request.SyllabusNodeId.HasValue
            ? area?.SyllabusNodes.FirstOrDefault(item => item.Id == request.SyllabusNodeId.Value)
            : null;

        if (journey is null || area is null || (request.SyllabusNodeId.HasValue && node is null))
        {
            Notification.CreateNotification(FlashcardTrace.Register, "Conteudo associado ao flashcard nao encontrado.");
            return null;
        }

        if (request.Model == "Verdadeiro ou falso" && !request.CorrectAnswer.HasValue)
        {
            Notification.CreateNotification(FlashcardTrace.Register, "Informe se a afirmacao e verdadeira ou falsa.");
            return null;
        }

        if (request.Model == "Omissão de palavras" &&
            (!request.Front.Contains("{{") || !request.Front.Contains("}}")))
        {
            Notification.CreateNotification(FlashcardTrace.Register,
                "Marque ao menos uma omissao entre chaves duplas.");
            return null;
        }

        var collection = await flashcardRepository.FindCollectionAsync(
            credential.UserId, journey.Id, area.Id, request.SyllabusNodeId);
        var card = mapper.DtoRegisterToDomain(collection?.Id ?? 1, request);
        if (!await EntityValidationAsync(card))
            return null;

        if (collection is null)
        {
            collection = new FlashCollection
            {
                UserId = credential.UserId,
                JourneyId = journey.Id,
                KnowledgeAreaId = area.Id,
                SyllabusNodeId = request.SyllabusNodeId,
                Title = (node?.Title ?? area.Title)[..Math.Min((node?.Title ?? area.Title).Length, 180)]
            };
            if (!await flashcardRepository.SaveCollectionAsync(collection))
            {
                Notification.CreateNotification(FlashcardTrace.Register,
                    "Nao foi possivel criar a colecao de flashcards.");
                return null;
            }
        }

        card.FlashCollectionId = collection.Id;
        if (!await flashcardRepository.SaveCardAsync(card))
        {
            Notification.CreateNotification(FlashcardTrace.Register, "Nao foi possivel cadastrar o flashcard.");
            return null;
        }

        GenerateLogger(EUserAction.Save, FlashcardTrace.Register, credential.UserId, card.Id.ToString());
        return mapper.DomainToDtoResponse(card, collection);
    }

    public async Task<FlashcardResponse?> RecallAsync(FlashcardRecallRequest request, UserCredential credential)
    {
        if (!Enum.IsDefined(request.Grade))
        {
            Notification.CreateNotification(FlashcardTrace.Recall, "Avaliacao invalida.");
            return null;
        }

        var card = await flashcardRepository.FindCardAsync(request.CardId, credential.UserId);
        if (card is null)
        {
            Notification.CreateNotification(FlashcardTrace.Recall, "Flashcard nao encontrado.");
            return null;
        }

        var previousInterval = card.IntervalDays;
        var newInterval = request.Grade switch
        {
            ERecallGrade.Again => 1,
            ERecallGrade.Hard => Math.Max(1, (int)Math.Round(Math.Max(1, previousInterval) * 1.2)),
            ERecallGrade.Good when previousInterval == 0 => 1,
            ERecallGrade.Good when previousInterval == 1 => 3,
            ERecallGrade.Good => Math.Max(1, (int)Math.Round(previousInterval * card.EaseFactor)),
            ERecallGrade.Easy when previousInterval == 0 => 4,
            _ => Math.Max(1, (int)Math.Round(previousInterval * card.EaseFactor * 1.3m))
        };
        card.EaseFactor = request.Grade switch
        {
            ERecallGrade.Again => Math.Max(1.3m, card.EaseFactor - .2m),
            ERecallGrade.Hard => Math.Max(1.3m, card.EaseFactor - .15m),
            ERecallGrade.Easy => Math.Min(3.5m, card.EaseFactor + .15m),
            _ => card.EaseFactor
        };
        card.IntervalDays = newInterval;
        card.NextReviewOn = CurrentDate().AddDays(newInterval);
        card.LastUpdateDate = DateTimeOffset.UtcNow;
        var recall = new MemoryRecall
        {
            MemoryCardId = card.Id,
            Grade = request.Grade,
            AnsweredAt = DateTimeOffset.UtcNow,
            PreviousIntervalDays = previousInterval,
            NewIntervalDays = newInterval
        };
        if (!await flashcardRepository.SaveRecallAsync(card, recall))
        {
            Notification.CreateNotification(FlashcardTrace.Recall, "Nao foi possivel salvar a revisao.");
            return null;
        }

        GenerateLogger(EUserAction.Update, FlashcardTrace.Recall, credential.UserId, card.Id.ToString());
        return mapper.DomainToDtoResponse(card, card.Collection);
    }

    public async Task<FlashcardResponse?> UpdateAsync(FlashcardUpdateRequest request, UserCredential credential)
    {
        var card = await flashcardRepository.FindCardAsync(request.Id, credential.UserId);
        if (card is null)
        {
            Notification.CreateNotification(FlashcardTrace.Update, "Flashcard nao encontrado.");
            return null;
        }

        if (request.Model == "Verdadeiro ou falso" && !request.CorrectAnswer.HasValue)
        {
            Notification.CreateNotification(FlashcardTrace.Update, "Informe se a afirmacao e verdadeira ou falsa.");
            return null;
        }

        if (request.Model == "Omissão de palavras" &&
            (!request.Front.Contains("{{") || !request.Front.Contains("}}")))
        {
            Notification.CreateNotification(FlashcardTrace.Update, "Marque ao menos uma omissao entre chaves duplas.");
            return null;
        }

        card.Model = request.Model.Trim();
        card.Type = request.Type.Trim();
        card.Front = request.Front.Trim();
        card.Back = request.Back.Trim();
        card.CorrectAnswer = request.Model == "Verdadeiro ou falso" ? request.CorrectAnswer : null;
        card.LastUpdateDate = DateTimeOffset.UtcNow;
        if (!await EntityValidationAsync(card) || !await flashcardRepository.UpdateCardAsync(card))
        {
            Notification.CreateNotification(FlashcardTrace.Update, "Nao foi possivel editar o flashcard.");
            return null;
        }

        GenerateLogger(EUserAction.Update, FlashcardTrace.Update, credential.UserId, card.Id.ToString());
        return mapper.DomainToDtoResponse(card, card.Collection);
    }

    public async Task<bool> DeleteAsync(long id, UserCredential credential)
    {
        var card = await flashcardRepository.FindCardAsync(id, credential.UserId);
        if (card is null)
            return Notification.CreateNotification(FlashcardTrace.Delete, "Flashcard nao encontrado.");
        if (!await flashcardRepository.DeleteCardAsync(card))
            return Notification.CreateNotification(FlashcardTrace.Delete, "Nao foi possivel excluir o flashcard.");
        GenerateLogger(EUserAction.Delete, FlashcardTrace.Delete, credential.UserId, id.ToString());
        return true;
    }

    private static DateOnly CurrentDate()
    {
        var zone = TimeZoneInfo.FindSystemTimeZoneById(
            OperatingSystem.IsWindows() ? "E. South America Standard Time" : "America/Sao_Paulo");
        return DateOnly.FromDateTime(TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow, zone));
    }
}