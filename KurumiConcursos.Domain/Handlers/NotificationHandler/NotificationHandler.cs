using KurumiConcursos.Domain.Interface;

namespace KurumiConcursos.Domain.Handlers.NotificationHandler;

public sealed class NotificationHandler : INotificationHandler
{
    private readonly List<DomainNotification> _notifications;

    public NotificationHandler()
    {
        _notifications = new List<DomainNotification>();
    }

    public List<DomainNotification> GetNotifications() => _notifications;

    public bool HasNotification() => _notifications.Count != 0;

    public void CreateNotifications(IEnumerable<DomainNotification> notifications) =>
        _notifications.AddRange(notifications);
    public void CreateNotification(DomainNotification notification) => _notifications.Add(notification);

    public void DeleteNotification(DomainNotification notification) => _notifications.Remove(notification);

    public bool CreateNotification(string key, string value)
    {
        _notifications.Add(new DomainNotification(key, value));
        return false;
    }
}
