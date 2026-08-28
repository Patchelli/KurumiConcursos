using FluentValidation;
using KurumiConcursos.ApplicationService.DataTransferObjects.JourneyDtos.Request;
using KurumiConcursos.ApplicationService.DataTransferObjects.JourneyDtos.Response;
using KurumiConcursos.ApplicationService.Interfaces.MapperContracts;
using KurumiConcursos.ApplicationService.Interfaces.ServiceContracts;
using KurumiConcursos.Domain.Entities;
using KurumiConcursos.Domain.Interface;
using KurumiConcursos.Domain.ValueObjects;
using KurumiConcursos.Infra.Interfaces.RepositoryContracts;

namespace KurumiConcursos.ApplicationService.Services.JourneyServices;

public sealed class JourneyCommandService(
    IJourneyRepository journeyRepository,
    IJourneyMapper journeyMapper,
    IValidate<ExamJourney> journeyValidation,
    IValidate<KnowledgeArea> knowledgeAreaValidation,
    IValidate<SyllabusNode> syllabusNodeValidation)
    : IJourneyCommandService
{
    public async Task<JourneyRegisterResponse?> RegisterAsync(
        SaveJourneyStructureRequest request,
        UserCredential userCredential)
    {
        var journey = journeyMapper.DtoRegisterToDomain(
            userCredential.UserId,
            request with { Journey = request.Journey with { Id = null } });

        await ValidateEntityAsync(journeyValidation, journey);
        foreach (var area in journey.KnowledgeAreas)
        {
            await ValidateEntityAsync(knowledgeAreaValidation, area);
            foreach (var node in area.SyllabusNodes)
                await ValidateEntityAsync(syllabusNodeValidation, node);
        }

        if (!await journeyRepository.SaveAsync(journey))
            return null;

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
            return false;

        return await journeyRepository.DeleteAsync(journey);
    }

    public async Task<bool> UpdateAsync(SaveJourneyStructureRequest request, UserCredential userCredential)
    {
        if (request.Journey.Id is null) return false;
        var journey = await journeyRepository.FindByIdAsync(request.Journey.Id.Value, userCredential.UserId,
            CancellationToken.None, includeStructure: true, tracking: true);
        if (journey is null) return false;

        journeyMapper.DtoUpdateToDomain(journey, request.Journey);
        var replacement = journeyMapper.DtoRegisterToDomain(userCredential.UserId, request);
        journey.KnowledgeAreas.Clear();
        journey.KnowledgeAreas = replacement.KnowledgeAreas;

        await ValidateEntityAsync(journeyValidation, journey);
        foreach (var area in journey.KnowledgeAreas)
        {
            area.JourneyId = journey.Id;
            await ValidateEntityAsync(knowledgeAreaValidation, area);
            foreach (var node in area.SyllabusNodes)
                await ValidateEntityAsync(syllabusNodeValidation, node);
        }
        return await journeyRepository.UpdateAsync(journey);
    }

    public async Task<bool> AddAreaAsync(SaveKnowledgeAreaRequest request, UserCredential userCredential)
    {
        var journey = await journeyRepository.FindByIdAsync(request.JourneyId, userCredential.UserId, CancellationToken.None);
        if (journey is null) return false;
        var area = new KnowledgeArea { JourneyId = journey.Id, Title = request.Title.Trim().ToUpperInvariant(), Order = request.Order, Weight = request.Weight, ExpectedQuestions = request.ExpectedQuestions };
        await ValidateEntityAsync(knowledgeAreaValidation, area);
        return await journeyRepository.SaveAreaAsync(area);
    }

    public async Task<bool> DeleteAreaAsync(long id, UserCredential userCredential)
    {
        var area = await journeyRepository.FindAreaAsync(id, userCredential.UserId, CancellationToken.None, true);
        return area is not null && await journeyRepository.DeleteAreaAsync(area);
    }

    public async Task<bool> AddNodeAsync(SaveSyllabusNodeRequest request, UserCredential userCredential)
    {
        var area = await journeyRepository.FindAreaAsync(request.KnowledgeAreaId, userCredential.UserId, CancellationToken.None);
        if (area is null) return false;
        if (request.ParentId.HasValue && !area.SyllabusNodes.Any(node => node.Id == request.ParentId.Value)) return false;
        var node = new SyllabusNode { KnowledgeAreaId = area.Id, ParentId = request.ParentId, Title = request.Title.Trim(), Order = request.Order };
        await ValidateEntityAsync(syllabusNodeValidation, node);
        return await journeyRepository.SaveNodeAsync(node);
    }

    public async Task<bool> DeleteNodeAsync(long id, UserCredential userCredential)
    {
        var node = await journeyRepository.FindNodeAsync(id, userCredential.UserId, CancellationToken.None, true);
        return node is not null && await journeyRepository.DeleteNodeAsync(node);
    }

    private static async Task ValidateEntityAsync<T>(IValidate<T> validator, T entity)
        where T : class
    {
        var response = await validator.ValidationAsync(entity);
        if (response.Valid)
            return;

        throw new ValidationException(string.Join("; ",
            response.Errors.Select(error => $"{error.Key}: {error.Value}")));
    }
}
