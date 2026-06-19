using Microsoft.AspNetCore.SignalR;

namespace Neighborhood.Services.API.Hubs
{
    public class SupportChatHub : Hub
    {
        // Join ticket room
        public async Task JoinTicket(string ticketId)
        {
            await Groups.AddToGroupAsync(
                Context.ConnectionId,
                $"ticket-{ticketId}");
        }

        // Leave ticket room
        public async Task LeaveTicket(string ticketId)
        {
            await Groups.RemoveFromGroupAsync(
                Context.ConnectionId,
                $"ticket-{ticketId}");
        }

        // Broadcast message to everyone in the ticket room
        public async Task SendMessage(
            string ticketId,
            object message)
        {
            await Clients.Group($"ticket-{ticketId}")
                .SendAsync("ReceiveMessage", message);
        }
    }
}