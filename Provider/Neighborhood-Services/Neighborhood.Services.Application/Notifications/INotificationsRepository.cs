using Neighborhood.Services.Application.Shared;
using Neighborhood.Services.Domain.Notifications;
using System;
using System.Collections.Generic;
using System.Text;

namespace Neighborhood.Services.Application.Notifications
{
    public interface INotificationsRepository : IGenericRepository<Notification, int>
    {
        public Task<List<Notification>> GetAllAsync(string currentUserId);

        public Task<List<Notification>> GetUnRead(string currentUserId);

        public Task<List<Notification>> GetRead(string currentUserId);

        public Task MarkAllAsReadAsync(string userId);

        public  Task MarkAsReadAsync(int notificationId);



    }
}
