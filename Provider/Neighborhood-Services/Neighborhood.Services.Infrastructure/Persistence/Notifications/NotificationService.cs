//using Microsoft.AspNetCore.SignalR;
//using Neighborhood.Services.Application.Notifications.Services;
//using Neighborhood.Services.
//using System;
//using System.Collections.Generic;
//using System.Text;

//namespace Neighborhood.Services.Infrastructure.Persistence.Notifications
//{
//    public class NotificationService : INotificationService
//    {
//        private readonly IHubContext<NotificationsHub> _hubContext;

//        public NotificationService(IHubContext<NotificationHub> hubContext)
//        {
//            _hubContext = hubContext;
//        }
//        public Task SendNotificationAsync(string message)
//        {
//            throw new NotImplementedException();
//        }

//        public Task SendNotificationToAdmin(string message)
//        {
//            throw new NotImplementedException();
//        }

//        public Task SendNotificationToUser(string userId, string message)
//        {
//            throw new NotImplementedException();
//        }

//        public Task SendRoleBasedNotificationAsync(string message, string role, string? recipientUserId = null)
//        {
//            throw new NotImplementedException();
//        }
//    }
//}
