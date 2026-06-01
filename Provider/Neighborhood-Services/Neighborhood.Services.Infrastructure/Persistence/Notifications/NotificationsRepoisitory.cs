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

namespace Neighborhood.Services.Infrastructure.Persistence.Notifications
{
    public class NotificationsRepoisitory: GenericRepository<Message, int>,INotificationsRepository
    {
        public NotificationsRepoisitory(ApplicationDbContext context) : base(context) { }

        IQueryable<Notification> IGenericRepository<Notification, int>.Table => throw new NotImplementedException();

        public Task AddAsync(Notification entity)
        {
            throw new NotImplementedException();
        }

        public Task<IEnumerable<Notification>> GetByConditionAsync(Expression<Func<Notification, bool>> expression, string? includeProperties = null, bool trackChanges = true)
        {
            throw new NotImplementedException();
        }

        public Task UpdateAsync(Notification entity)
        {
            throw new NotImplementedException();
        }

        Task<IReadOnlyList<Notification>> IGenericRepository<Notification, int>.GetAllAsync()
        {
            throw new NotImplementedException();
        }

        Task<Notification> IGenericRepository<Notification, int>.GetByIdAsync(int id)
        {
            throw new NotImplementedException();
        }
    }
}
