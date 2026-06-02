
using Neighborhood.Services.Application.Notifications.Services;
using Microsoft.AspNetCore.SignalR;

namespace Neighborhood.Services.Infrastructure.Services.NotificationService
{
    public class NotificationService : INotificationService
    {
        private readonly IHubContext<NotificationHub> _hubContext;

        public NotificationService(IHubContext<NotificationHub> hubContext)
        {
            _hubContext = hubContext;
        }
        public async Task SendNotificationAsync(string message)
        {
            await _hubContext.Clients.All.SendAsync("ReceiveNotification", new { message });

        }

        public async Task SendNotificationToAdmin(string message)
        {
            await _hubContext.Clients.Group("Staff").SendAsync("ReceiveNotification", new { message });
        }

        public async Task SendNotificationToAdminIdentity(string message)
        {
            await _hubContext.Clients.Group("Admin").SendAsync("ReceiveNotification", new { message });
        }

        public async Task SendNotificationToCustomer(string message)
        {
            await _hubContext.Clients.Group("Customer").SendAsync("ReceiveNotification", new { message });
        }

        public async Task SendNotificationToTechnician(string message)
        {
            await _hubContext.Clients.Group("Technician").SendAsync("ReceiveNotification", new { message });
        }

        //Customer,
        //Technician,
        //Staff

        public async Task SendNotificationToUser(string userId, string message)
        {
            await _hubContext.Clients.Group($"business-{userId}").SendAsync("ReceiveNotification", new { message });

        }

        public async Task SendRoleBasedNotificationAsync(string message, string role, string? recipientUserId = null)
        {
            switch (role)
            {
                case "Staff":
                    await SendNotificationToAdmin(message);
                    break;

                case "Technician":
                case "Customer":
                    if (!string.IsNullOrEmpty(recipientUserId))
                        await SendNotificationToUser(recipientUserId, message);
                    break;

                default:
                    await SendNotificationAsync(message);
                    break;
            }
        }
    }
}
