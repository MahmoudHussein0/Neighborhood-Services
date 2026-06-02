using System;
using System.Collections.Generic;
using System.Text;

namespace Neighborhood.Services.Application.Notifications.Services
{
    public interface INotificationService
    {
        Task SendNotificationAsync(string message);
        Task SendNotificationToAdmin(string message);
        Task SendNotificationToUser(string userId, string message);

        Task SendRoleBasedNotificationAsync(string message, string role, string? recipientUserId = null);
    }
}
