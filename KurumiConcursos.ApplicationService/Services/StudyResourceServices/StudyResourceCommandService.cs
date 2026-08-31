using KurumiConcursos.ApplicationService.DataTransferObjects.StudyResourceDtos.Request;
using KurumiConcursos.ApplicationService.DataTransferObjects.StudyResourceDtos.Response;
using KurumiConcursos.ApplicationService.Interfaces.MapperContracts;
using KurumiConcursos.ApplicationService.Interfaces.ServiceContracts;
using KurumiConcursos.ApplicationService.Traces;
using KurumiConcursos.Domain.Entities;
using KurumiConcursos.Domain.Enums;
using KurumiConcursos.Domain.Interface;
using KurumiConcursos.Domain.ValueObjects;
using KurumiConcursos.Infra.Interfaces.RepositoryContracts;

namespace KurumiConcursos.ApplicationService.Services.StudyResourceServices;

public sealed class StudyResourceCommandService(
    IStudyResourceRepository studyResourceRepository,
    IStudyResourceMapper studyResourceMapper,
    IValidate<StudyResource> studyResourceValidation,
    INotificationHandler notificationHandler,
    ILoggerHandler logger)
    : ServiceBase<StudyResource>(notificationHandler, studyResourceValidation, logger),
        IStudyResourceCommandService
{
    private const string EntityName = "material de estudo";

    public async Task<StudyResourceResponse?> RegisterAsync(
        StudyResourceRegisterRequest request,
        UserCredential credential)
    {
        if (request.JourneyId <= 0 || string.IsNullOrWhiteSpace(request.Url))
        {
            Notification.CreateNotification(
                StudyResourceTrace.Register,
                "Jornada e URL do material sao obrigatorias.");
            return null;
        }

        var resource = studyResourceMapper.DtoRegisterToDomain(credential.UserId, request);

        if (!await EntityValidationAsync(resource))
            return null;

        if (!await studyResourceRepository.SaveAsync(resource))
        {
            Notification.CreateNotification(
                StudyResourceTrace.Register,
                $"Nao foi possivel cadastrar o {EntityName}.");
            return null;
        }

        GenerateLogger(
            EUserAction.Save,
            StudyResourceTrace.Register,
            credential.UserId,
            resource.Id.ToString());

        return studyResourceMapper.DomainToDtoResponse(resource);
    }

    public async Task<bool> DeleteAsync(long id, UserCredential credential)
    {
        if (id <= 0)
            return Notification.CreateNotification(
                StudyResourceTrace.Delete,
                $"{EntityName} invalido.");

        var resource = await studyResourceRepository.FindByPredicateAsync(
            item => item.Id == id && item.UserId == credential.UserId,
            asNoTracking: false);

        if (resource is null)
            return Notification.CreateNotification(
                StudyResourceTrace.Delete,
                $"{EntityName} nao encontrado.");

        if (!await studyResourceRepository.DeleteAsync(resource))
            return Notification.CreateNotification(
                StudyResourceTrace.Delete,
                $"Nao foi possivel excluir o {EntityName}.");

        GenerateLogger(
            EUserAction.Delete,
            StudyResourceTrace.Delete,
            credential.UserId,
            resource.Id.ToString());

        return true;
    }
}