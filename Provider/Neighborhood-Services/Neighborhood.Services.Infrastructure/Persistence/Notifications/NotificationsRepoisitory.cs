using Neighborhood.Services.Domain.Message;
using Neighborhood.Services.Infrastructure.Persistence.Context;
using Neighborhood.Services.Infrastructure.Shared;
using System;
using System.Collections.Generic;
using System.Text;
using Neighborhood.Services.Infrastructure.Persistence.Context;
using Neighborhood.Services.Application.Notifications;
using Neighborhood.Services.Domain.Notifications;
using Neighborhood.Services.Application.Shared;
using System.Linq.Expressions;
using Microsoft.EntityFrameworkCore;

namespace Neighborhood.Services.Infrastructure.Persistence.Notifications
{
    public class NotificationsRepoisitory : GenericRepository<Notification, int>, INotificationsRepository
    {
        public NotificationsRepoisitory(ApplicationDbContext context) : base(context) { }

        public async Task<List<Notification>> GetAllAsync(string currentUserId)
        {
            return await _context.Notifications
            .Where(n => n.UserId == currentUserId)
            .OrderByDescending(n => n.createdAt)
            .ToListAsync();
        }

        public async Task<List<Notification>> GetUnRead(string currentUserId)
        {
            return await _context.Notifications
                .Where(n =>n.isRead==false).
                OrderByDescending(n => n.createdAt).
                ToListAsync();
        }

        public async Task<List<Notification>> GetRead(string currentUserId)
        {
            return await _context.Notifications
                .Where(n => n.isRead == true).
                OrderByDescending(n => n.createdAt).
                ToListAsync();

        }


        public async Task MarkAllAsReadAsync(string userId)
        {
            var notifications = await _context.Notifications.Where(n => n.UserId == userId && !n.isRead).ToListAsync();
            notifications.ForEach(n => n.isRead = true);
            await _context.SaveChangesAsync();
        }

        public async Task MarkAsReadAsync(int notificationId)
        {
            var notification = await _context.Notifications.FindAsync(notificationId);
            if (notification != null)
            {
                notification.isRead = true;
                await _context.SaveChangesAsync();
            }
        }
    }
}
