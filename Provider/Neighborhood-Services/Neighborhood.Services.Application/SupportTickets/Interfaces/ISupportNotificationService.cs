using Neighborhood.Services.Application.SupportTickets.DTOs;
using System;
using System.Collections.Generic;
using System.Text;

namespace Neighborhood.Services.Application.SupportTickets.Interfaces
{
    public interface ISupportNotificationService
    {
        Task NotifyNewMessageAsync(
            int ticketId,
            SupportMessageDto message);
    }
}
