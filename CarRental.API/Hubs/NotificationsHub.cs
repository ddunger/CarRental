using CarRental.Domain.Constants;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.SignalR;

namespace CarRental.API.Hubs
{
    [Authorize]
    public class NotificationsHub : Hub
    {
        public const string StaffGroup = "staff";

        public override async Task OnConnectedAsync()
        {
            if (Context.User is not null &&
                (Context.User.IsInRole(Roles.Admin) || Context.User.IsInRole(Roles.Manager)))
            {
                await Groups.AddToGroupAsync(Context.ConnectionId, StaffGroup);
            }

            await base.OnConnectedAsync();
        }
    }
}