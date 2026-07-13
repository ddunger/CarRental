namespace CarRental.Application.SignalR
{
    public interface IStaffNotificationPublisher
    {
        Task PublishRentalOverdueAsync(RentalOverdueNotification notification, CancellationToken cancellationToken);
    }
}
