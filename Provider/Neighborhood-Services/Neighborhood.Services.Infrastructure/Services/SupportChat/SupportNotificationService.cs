using Microsoft.AspNetCore.SignalR;
using Neighborhood.Services.Application.SupportTickets.DTOs;
using Neighborhood.Services.Application.SupportTickets.Interfaces;
using System;
using System.Collections.Generic;
using System.Text;

namespace Neighborhood.Services.Infrastructure.Services.SupportChat
{
    public class SupportNotificationService
       : ISupportNotificationService
    {
        private readonly IHubContext<SupportChatHub> _hubContext;

        public SupportNotificationService(
            IHubContext<SupportChatHub> hubContext)
        {
            _hubContext = hubContext;
        }

        public async Task NotifyNewMessageAsync(
            int ticketId,
            SupportMessageDto message)
        {
            await _hubContext.Clients
                .Group($"ticket-{ticketId}")
                .SendAsync("ReceiveMessage", message);
        }
    }
}
