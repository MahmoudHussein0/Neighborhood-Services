using Neighborhood.Services.Domain.Message;
using Neighborhood.Services.Infrastructure.Shared;
using System;
using System.Collections.Generic;
using System.Text;

namespace Neighborhood.Services.Infrastructure.Persistence.Notifications
{
    public class NotificationsRepoisitory: GenericRepository<Message, int>
    {
        public NotificationsRepoisitory(ApplicationDbContext context) : base(context) { }

    }
}
