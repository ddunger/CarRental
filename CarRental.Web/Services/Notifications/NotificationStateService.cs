using CarRental.Web.Dtos.Notifications;

namespace CarRental.Web.Services.Notifications
{
    public class NotificationStateService
    {
        private readonly List<RentalOverdueNotification> _notifications = [];

        public IReadOnlyList<RentalOverdueNotification> Notifications => _notifications;
        public int UnreadCount { get; private set; }

        public event Action? Changed;

        public void Add(RentalOverdueNotification notification)
        {
            _notifications.Insert(0, notification);   // newest first
            UnreadCount++;
            Changed?.Invoke();
        }

        public void MarkAllRead()
        {
            UnreadCount = 0;
            Changed?.Invoke();
        }

        public void Clear()
        {
            _notifications.Clear();
            UnreadCount = 0;
            Changed?.Invoke();
        }
    }
}