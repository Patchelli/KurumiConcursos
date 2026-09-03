using KurumiConcursos.ApplicationService.DataTransferObjects.JourneyDtos.Request;
using KurumiConcursos.ApplicationService.DataTransferObjects.JourneyDtos.Response;
using KurumiConcursos.ApplicationService.Interfaces.MapperContracts;
using KurumiConcursos.ApplicationService.Interfaces.ServiceContracts;
using KurumiConcursos.ApplicationService.Traces;
using KurumiConcursos.Domain.Entities;
using KurumiConcursos.Domain.Enums;
using KurumiConcursos.Domain.Handlers.NotificationHandler;
using KurumiConcursos.Domain.Interface;
using KurumiConcursos.Domain.ValueObjects;
using KurumiConcursos.Infra.Interfaces.RepositoryContracts;

namespace KurumiConcursos.ApplicationService.Services.JourneyServices;

public sealed class JourneyCommandService(
    IJourneyRepository journeyRepository,
    IJourneyMapper journeyMapper,
    IValidate<ExamJourney> journeyValidation,
    IValidate<KnowledgeArea> knowledgeAreaValidation,
    IValidate<SyllabusNode> syllabusNodeValidation,
    INotificationHandler notificationHandler,
    ILoggerHandler logger)
    : ServiceBase<ExamJourney>(notificationHandler, journeyValidation, logger),
        IJourneyCommandService
{
    public async Task<JourneyRegisterResponse?> RegisterAsync(
        JourneyRegisterRequest request,
        UserCredential userCredential)
    {
        var journey = journeyMapper.DtoRegisterToDomain(userCredential.UserId, request);

        if (!await EntityValidationAsync(journey)) return null;
        foreach (var area in journey.KnowledgeAreas)
        {
            if (!await ValidateEntityAsync(knowledgeAreaValidation, area)) return null;
            foreach (var node in area.SyllabusNodes)
                if (!await ValidateEntityAsync(syllabusNodeValidation, node))
                    return null;
        }

        if (!await journeyRepository.SaveAsync(journey))
        {
            Notification.CreateNotification(JourneyTrace.Register, "Não foi possível cadastrar a jornada.");
            return null;
        }

        GenerateLogger(EUserAction.Save, JourneyTrace.Register, userCredential.UserId, journey);

        return new JourneyRegisterResponse(journey.Id);
    }

    public async Task<bool> DeleteRegisterAsync(long id, UserCredential userCredential)
    {
        var journey = await journeyRepository.FindByIdAsync(
            id,
            userCredential.UserId,
            CancellationToken.None,
            tracking: true);

        if (journey is null)
            return Notification.CreateNotification(JourneyTrace.Delete, "Jornada não encontrada.");

        if (!await journeyRepository.DeleteAsync(journey))
            return Notification.CreateNotification(JourneyTrace.Delete, "Não foi possível excluir a jornada.");

        GenerateLogger(EUserAction.Delete, JourneyTrace.Delete, userCredential.UserId, journey.Id.ToString());
        return true;
    }

    public async Task<bool> UpdateAsync(JourneyUpdateRequest request, UserCredential userCredential)
    {
        var journey = await journeyRepository.FindByIdAsync(request.Id, userCredential.UserId,
            CancellationToken.None, includeStructure: true, tracking: true);
        if (journey is null)
            return Notification.CreateNotification(JourneyTrace.Update, "Jornada não encontrada.");

        journeyMapper.DtoUpdateToDomain(journey, request);

        if (!await EntityValidationAsync(journey)) return false;
        foreach (var area in journey.KnowledgeAreas)
        {
            area.Journey = journey;
            area.JourneyId = journey.Id;
            if (!await ValidateEntityAsync(knowledgeAreaValidation, area)) return false;
            foreach (var node in area.SyllabusNodes)
                if (!await ValidateEntityAsync(syllabusNodeValidation, node))
                    return false;
        }

        if (!await journeyRepository.UpdateAsync(journey))
            return Notification.CreateNotification(JourneyTrace.Update, "Não foi possível atualizar a jornada.");

        GenerateLogger(EUserAction.Update, JourneyTrace.Update, userCredential.UserId, journey.Id.ToString());
        return true;
    }

    public async Task<bool> AddAreaAsync(KnowledgeAreaRegisterRequest request, UserCredential userCredential)
    {
        var journey =
            await journeyRepository.FindByIdAsync(request.JourneyId, userCredential.UserId, CancellationToken.None);
        if (journey is null)
            return Notification.CreateNotification(JourneyTrace.AddKnowledgeArea, "Jornada não encontrada.");
        var area = new KnowledgeArea
        {
            JourneyId = journey.Id, Title = request.Title.Trim().ToUpperInvariant(), Order = request.Order,
            Weight = request.Weight, ExpectedQuestions = request.ExpectedQuestions
        };
        if (!await ValidateEntityAsync(knowledgeAreaValidation, area)) return false;
        if (!await journeyRepository.SaveAreaAsync(area))
            return Notification.CreateNotification(JourneyTrace.AddKnowledgeArea,
                "Não foi possível cadastrar a área de conhecimento.");

        GenerateLogger(EUserAction.Save, JourneyTrace.AddKnowledgeArea, userCredential.UserId, area);
        return true;
    }

    public async Task<bool> UpdateAreaAsync(KnowledgeAreaRegisterRequest request, UserCredential userCredential)
    {
        if (!request.Id.HasValue)
            return Notification.CreateNotification(JourneyTrace.UpdateKnowledgeArea, "Id da área é obrigatório.");

        var area = await journeyRepository.FindAreaAsync(request.Id.Value, userCredential.UserId,
            CancellationToken.None, tracking: true);
        if (area is null)
            return Notification.CreateNotification(JourneyTrace.UpdateKnowledgeArea,
                "Área de conhecimento não encontrada.");

        area.Title = request.Title.Trim().ToUpperInvariant();
        area.Order = request.Order;
        area.Weight = request.Weight;
        area.ExpectedQuestions = request.ExpectedQuestions;

        if (!await ValidateEntityAsync(knowledgeAreaValidation, area)) return false;
        if (!await journeyRepository.UpdateAreaAsync(area))
            return Notification.CreateNotification(JourneyTrace.UpdateKnowledgeArea,
                "Não foi possível atualizar a área de conhecimento.");

        GenerateLogger(EUserAction.Update, JourneyTrace.UpdateKnowledgeArea, userCredential.UserId, area);
        return true;
    }

    public async Task<bool> DeleteAreaAsync(long id, UserCredential userCredential)
    {
        var area = await journeyRepository.FindAreaAsync(id, userCredential.UserId, CancellationToken.None, true);
        if (area is null)
            return Notification.CreateNotification(JourneyTrace.DeleteKnowledgeArea,
                "Área de conhecimento não encontrada.");
        if (!await journeyRepository.DeleteAreaAsync(area))
            return Notification.CreateNotification(JourneyTrace.DeleteKnowledgeArea,
                "Não foi possível excluir a área de conhecimento.");

        GenerateLogger(EUserAction.Delete, JourneyTrace.DeleteKnowledgeArea, userCredential.UserId, area.Id.ToString());
        return true;
    }

    public async Task<bool> AddNodeAsync(SyllabusNodeRegisterRequest request, UserCredential userCredential)
    {
        var area = await journeyRepository.FindAreaAsync(request.KnowledgeAreaId, userCredential.UserId,
            CancellationToken.None);
        if (area is null)
            return Notification.CreateNotification(JourneyTrace.AddSyllabusNode,
                "Área de conhecimento não encontrada.");
        if (request.ParentId.HasValue && !area.SyllabusNodes.Any(node => node.Id == request.ParentId.Value))
            return Notification.CreateNotification(JourneyTrace.AddSyllabusNode, "Tópico pai não encontrado.");
        var node = new SyllabusNode
        {
            KnowledgeAreaId = area.Id, ParentId = request.ParentId, Title = request.Title.Trim(), Order = request.Order
        };
        if (!await ValidateEntityAsync(syllabusNodeValidation, node)) return false;
        if (!await journeyRepository.SaveNodeAsync(node))
            return Notification.CreateNotification(JourneyTrace.AddSyllabusNode,
                "Não foi possível cadastrar o tópico.");

        GenerateLogger(EUserAction.Save, JourneyTrace.AddSyllabusNode, userCredential.UserId, node);
        return true;
    }

    public async Task<bool> UpdateNodeAsync(SyllabusNodeRegisterRequest request, UserCredential userCredential)
    {
        if (!request.Id.HasValue)
            return Notification.CreateNotification(JourneyTrace.UpdateSyllabusNode, "Id do tópico é obrigatório.");

        var node = await journeyRepository.FindNodeAsync(request.Id.Value, userCredential.UserId,
            CancellationToken.None, tracking: true);
        if (node is null)
            return Notification.CreateNotification(JourneyTrace.UpdateSyllabusNode, "Tópico não encontrado.");

        node.Title = request.Title.Trim();
        node.Order = request.Order;

        if (!await ValidateEntityAsync(syllabusNodeValidation, node)) return false;
        if (!await journeyRepository.UpdateNodeAsync(node))
            return Notification.CreateNotification(JourneyTrace.UpdateSyllabusNode,
                "Não foi possível atualizar o tópico.");

        GenerateLogger(EUserAction.Update, JourneyTrace.UpdateSyllabusNode, userCredential.UserId, node);
        return true;
    }

    public async Task<bool> DeleteNodeAsync(long id, UserCredential userCredential)
    {
        var node = await journeyRepository.FindNodeAsync(id, userCredential.UserId, CancellationToken.None, true);
        if (node is null)
            return Notification.CreateNotification(JourneyTrace.DeleteSyllabusNode, "Tópico não encontrado.");
        if (!await journeyRepository.DeleteNodeAsync(node))
            return Notification.CreateNotification(JourneyTrace.DeleteSyllabusNode,
                "Não foi possível excluir o tópico.");

        GenerateLogger(EUserAction.Delete, JourneyTrace.DeleteSyllabusNode, userCredential.UserId, node.Id.ToString());
        return true;
    }

    private async Task<bool> ValidateEntityAsync<T>(IValidate<T> validator, T entity)
        where T : class
    {
        var response = await validator.ValidationAsync(entity);
        if (!response.Valid)
            Notification.CreateNotifications(DomainNotification.CreateNotifications(response.Errors));
        return response.Valid;
    }
}