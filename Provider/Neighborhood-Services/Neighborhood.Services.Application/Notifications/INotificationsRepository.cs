using Neighborhood.Services.Application.Shared;
using Neighborhood.Services.Domain.Notifications;
using System;
using System.Collections.Generic;
using System.Text;

namespace Neighborhood.Services.Application.Notifications
{
    public interface INotificationsRepository: IGenericRepository<Notification, int>
    {
    }
}
