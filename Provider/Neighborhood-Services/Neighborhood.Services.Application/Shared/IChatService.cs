using Microsoft.AspNetCore.SignalR;
using Neighborhood.Services.Application.Messages.DTOs;
using System;
using System.Collections.Generic;
using System.Text;


namespace Neighborhood.Services.Application.Shared
{
    public interface IChatService
    {   public  Task SendPrivateMessage(string userId, string message);
        public  Task SendPrivateMessageDto(string userId, MessageCreatedDto message);
        public Task SendGroupMessage(string groupName, string message);

        public Task SendGroupMessage(string groupName, MessageCreatedDto message);

        public Task SendGroupMessageDto(string groupName, MessageCreatedDto message);
        public  Task SendBroadcastMessage(string message);
        public Task SendBroadcastMessageDto(MessageCreatedDto message);

    }
}
