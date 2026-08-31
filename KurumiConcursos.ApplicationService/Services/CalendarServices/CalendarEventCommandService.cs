using KurumiConcursos.ApplicationService.DataTransferObjects.CalendarEventDtos.Request;
using KurumiConcursos.ApplicationService.DataTransferObjects.CalendarEventDtos.Response;
using KurumiConcursos.ApplicationService.Interfaces.MapperContracts;
using KurumiConcursos.ApplicationService.Interfaces.ServiceContracts;
using KurumiConcursos.ApplicationService.Traces;
using KurumiConcursos.Domain.Entities;
using KurumiConcursos.Domain.Enums;
using KurumiConcursos.Domain.Interface;
using KurumiConcursos.Domain.ValueObjects;
using KurumiConcursos.Infra.Interfaces.RepositoryContracts;

namespace KurumiConcursos.ApplicationService.Services.CalendarServices;

public sealed class CalendarEventCommandService(
    ICalendarEventRepository calendarEventRepository,
    ICalendarEventMapper calendarEventMapper,
    IValidate<CalendarEvent> calendarEventValidation,
    INotificationHandler notificationHandler,
    ILoggerHandler logger)
    : ServiceBase<CalendarEvent>(notificationHandler, calendarEventValidation, logger),
        ICalendarEventCommandService
{
    private const string EntityName = "evento do calendário";

    public async Task<CalendarEventResponse?> RegisterAsync(
        CalendarEventRegisterRequest request,
        UserCredential credential)
    {
        if (!IsValidRequest(request.Title, request.Date, request.Type, CalendarEventTrace.Register))
            return null;

        var calendarEvent = calendarEventMapper.DtoRegisterToDomain(credential.UserId, request);

        if (!await EntityValidationAsync(calendarEvent))
            return null;

        if (!await calendarEventRepository.SaveAsync(calendarEvent))
        {
            Notification.CreateNotification(
                CalendarEventTrace.Register,
                $"Não foi possível cadastrar o {EntityName}.");
            return null;
        }

        GenerateLogger(
            EUserAction.Save,
            CalendarEventTrace.Register,
            credential.UserId,
            calendarEvent.Id.ToString());

        return calendarEventMapper.DomainToDtoResponse(calendarEvent);
    }

    public async Task<bool> UpdateAsync(
        CalendarEventUpdateRequest request,
        UserCredential credential)
    {
        if (request.Id <= 0 ||
            !IsValidRequest(request.Title, request.Date, request.Type, CalendarEventTrace.Update))
            return false;

        var calendarEvent = await calendarEventRepository.FindByPredicateAsync(
            item => item.Id == request.Id && item.UserId == credential.UserId,
            asNoTracking: false);

        if (calendarEvent is null)
        {
            return Notification.CreateNotification(
                CalendarEventTrace.Update,
                $"{EntityName} não encontrado.");
        }

        calendarEventMapper.DtoUpdateToDomain(calendarEvent, request);

        if (!await EntityValidationAsync(calendarEvent))
            return false;

        if (!await calendarEventRepository.UpdateAsync(calendarEvent))
        {
            return Notification.CreateNotification(
                CalendarEventTrace.Update,
                $"Não foi possível atualizar o {EntityName}.");
        }

        GenerateLogger(
            EUserAction.Update,
            CalendarEventTrace.Update,
            credential.UserId,
            calendarEvent.Id.ToString());

        return true;
    }

    public async Task<bool> DeleteAsync(long id, UserCredential credential)
    {
        if (id <= 0)
        {
            return Notification.CreateNotification(
                CalendarEventTrace.Delete,
                $"{EntityName} inválido.");
        }

        var calendarEvent = await calendarEventRepository.FindByPredicateAsync(
            item => item.Id == id && item.UserId == credential.UserId,
            asNoTracking: false);

        if (calendarEvent is null)
        {
            return Notification.CreateNotification(
                CalendarEventTrace.Delete,
                $"{EntityName} não encontrado.");
        }

        if (!await calendarEventRepository.DeleteAsync(calendarEvent))
        {
            return Notification.CreateNotification(
                CalendarEventTrace.Delete,
                $"Não foi possível excluir o {EntityName}.");
        }

        GenerateLogger(
            EUserAction.Delete,
            CalendarEventTrace.Delete,
            credential.UserId,
            calendarEvent.Id.ToString());

        return true;
    }

    private bool IsValidRequest(
        string? title,
        DateOnly date,
        ECalendarEventType type,
        string trace)
    {
        if (string.IsNullOrWhiteSpace(title))
        {
            Notification.CreateNotification(trace, "O título do evento é obrigatório.");
            return false;
        }

        if (date == default)
        {
            Notification.CreateNotification(trace, "A data do evento é obrigatória.");
            return false;
        }

        if (!Enum.IsDefined(type))
        {
            Notification.CreateNotification(trace, "O tipo do evento é inválido.");
            return false;
        }

        return true;
    }
}