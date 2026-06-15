using Neighborhood.Services.Application.Notifications.Push_inApp.DTOs;
using Neighborhood.Services.Domain.ApplicationUsers;
using System;
using System.Collections.Generic;
using System.Text;

namespace Neighborhood.Services.Application.Notifications.Services
{
    public interface INotificationService
    {
        public Task<PushNotificationDto> SendNotificationAsync(string message);
        public Task<PushNotificationDto> SendNotificationToAdmin(string message);
        public Task<PushNotificationDto> SendNotificationToUser(string userId, string message);

        public Task<PushNotificationDto> SendRoleBasedNotificationAsync(string message, ApplicationUserRole role, string? recipientUserId = null);
        public Task<PushNotificationDto> SendNotificationToTechnician(string mssg);

        public  Task<PushNotificationDto> SendNotificationToCustomer(string mssg);


    }
}
