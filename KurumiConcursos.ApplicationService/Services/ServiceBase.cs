using KurumiConcursos.Domain.Entities;
using KurumiConcursos.Domain.Enums;
using KurumiConcursos.Domain.Extensions;
using KurumiConcursos.Domain.Handlers.NotificationHandler;
using KurumiConcursos.Domain.Interface;

namespace KurumiConcursos.ApplicationService.Services;

public abstract class ServiceBase<T>(
    INotificationHandler notification,
    IValidate<T> validate,
    ILoggerHandler logger)
    where T : class
{
    protected readonly INotificationHandler Notification = notification;

    protected async Task<bool> EntityValidationAsync(T entity)
    {
        var validationResponse = await validate.ValidationAsync(entity);

        if (!validationResponse.Valid)
            Notification.CreateNotifications(DomainNotification.CreateNotifications(validationResponse.Errors));

        return validationResponse.Valid;
    }

    protected bool EntitiesValidation(List<T> entities)
    {
        foreach (var validationResponse in entities
                     .Select(validate.Validation)
                     .Where(validationResponse => !validationResponse.Valid))
        {
            Notification.CreateNotifications(DomainNotification.CreateNotifications(validationResponse.Errors));
        }

        return !Notification.HasNotification();
    }

    protected void GenerateLogger(EUserAction action, string eventDescription, Guid userId, string? entityId = null)
    {
        var log = new DomainLogger
        {
            Action = action,
            ActionDate = DateTime.Now.GetDateAndTimeInBrasilia(),
            Description = eventDescription,
            UserId = userId,
            EntityId = entityId
        };

        logger.CreateLogger(log);
    }

    protected void GenerateLogger(EUserAction action, string eventDescription, Guid userId, object entity)
    {
        var log = new DomainLogger
        {
            Action = action,
            ActionDate = DateTime.Now.GetDateAndTimeInBrasilia(),
            Description = eventDescription,
            UserId = userId,
            EntityId = null
        };

        logger.CreateLogger(log, entity);
    }
}