using Microsoft.AspNetCore.SignalR;
using Microsoft.Extensions.Logging;
using Neighborhood.Services.Application.Messages.DTOs;
using Neighborhood.Services.Application.Shared;
using Neighborhood.Services.Infrastructure.Services.NotificationService;
using System;
using System.Collections.Generic;
using System.Text;

namespace Neighborhood.Services.Infrastructure.Services.ChatService
{
    public class ChatService :IChatService
    {
        private readonly IHubContext<ChatHub> _hubContext;
     
        private readonly ILogger<ChatService> _logger;
        private readonly ICurrentUserService _current;

        public ChatService(IHubContext<ChatHub> hubContext, ILogger<ChatService> logger,ICurrentUserService current)
        {
            _hubContext= hubContext;
            _logger= logger;
            _current = current;
            
        }

        public async Task SendPrivateMessage(string userId, string message)
        {
            await _hubContext.Clients.User(userId).SendAsync("ReceiveMessage", message);
        }
        public async Task SendPrivateMessageDto(string userId, MessageCreatedDto message)
        {
            await _hubContext.Clients.User(userId).SendAsync("ReceiveMessage", message);
        }

        public async Task SendGroupMessage(string groupName, string message)
        {
            await _hubContext.Clients.Group(groupName).SendAsync("ReceiveMessage", new {content=message, senderId = _current.UserId});
        }

        public async Task SendGroupMessageDto(string groupName, MessageCreatedDto message)
        {
            await _hubContext.Clients.Group(groupName).SendAsync("ReceiveMessage", message);
        }

        public async Task SendBroadcastMessage(string message)
        {
            
            await _hubContext.Clients.All.SendAsync("ReceiveMessage", message);
        }

        public async Task SendBroadcastMessageDto(MessageCreatedDto message)
        {

            await _hubContext.Clients.All.SendAsync("ReceiveMessage", message);
        }

        //public async Task LeaveGroup(string connectionId, string groupName)
        //{
        //    await _hubContext.Groups.RemoveFromGroupAsync(connectionId, groupName);
        //}

        //public async Task JoinGroup(string connectionId, string groupName)
        //{
        //    await _hubContext.Groups.AddToGroupAsync(connectionId, groupName);
        //}

    }
}
