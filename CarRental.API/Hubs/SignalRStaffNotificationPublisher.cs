using CarRental.Application.Interfaces;
using CarRental.Application.SignalR;
using Microsoft.AspNetCore.SignalR;

namespace CarRental.API.Hubs
{
    public class SignalRStaffNotificationPublisher(
        IHubContext<NotificationsHub> hubContext) : IStaffNotificationPublisher
    {
        public Task PublishRentalOverdueAsync(RentalOverdueNotification notification, CancellationToken cancellationToken)
        {
            return hubContext.Clients.Group(NotificationsHub.StaffGroup)
                .SendAsync("RentalOverdue", notification, cancellationToken);
        }
    }
}