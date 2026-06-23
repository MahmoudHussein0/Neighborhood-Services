using Microsoft.AspNetCore.SignalR;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using MimeKit.Encodings;
using Neighborhood.Services.Application.Messages.DTOs;
using Neighborhood.Services.Application.Shared;
using NetTopologySuite.Index.HPRtree;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Text;

namespace Neighborhood.Services.Infrastructure.Services.ChatService
{
    public class ChatHub : Hub
    {
        private readonly IConfiguration _configuration;
        private readonly IChatService _chatService;

        public ChatHub(IConfiguration confg,IChatService chatService)
        {
            _configuration = confg;
            _chatService= chatService;
        }
        public override async Task OnConnectedAsync()
        {
            
            //var token = Context.GetHttpContext()?.Request.Query["access_token"].ToString();
            //if (string.IsNullOrEmpty(token))
            //{
            //    await Clients.Caller.SendAsync("Error", "Authentication failed: No token provided");
            //    Context.Abort();
            //    return;
            //}
            //if (!ValidateJwtToken(token))
            //{
            //    await Clients.Caller.SendAsync("Error", "Authentication failed: Invalid token");
            //    Context.Abort();
            //    return;
            //}
            await base.OnConnectedAsync();
        }
        private static readonly ConcurrentDictionary<string, HashSet<string>> _userGroups = new();

        public override async Task OnDisconnectedAsync(Exception? exception)
        {
            var connId = Context.ConnectionId;

            if (_userGroups.TryRemove(connId, out var groups))
            {
                // SignalR removes groups automatically, but we clean our tracking dict
                foreach (var group in groups)
                {
                    await Groups.RemoveFromGroupAsync(connId, group);
                    Console.WriteLine($"Connection {connId} left group {group}");
                }
            }

            await base.OnDisconnectedAsync(exception);
        }

        public async Task SendPrivateMessage(string userId, string message)
        {
            await Clients.User(userId).SendAsync("ReceiveMessage", message);
        }
        public async Task SendPrivateMessageDto(string userId, MessageCreatedDto message)
        {
            await Clients.User(userId).SendAsync("ReceiveMessage", message);
        }

        public async Task SendGroupMessage(string groupName, string message)
        {
            await Clients.Group(groupName).SendAsync("ReceiveMessage", message);
        }

        public async Task SendGroupMessageExceptforCaller(string groupName, string message)
        {
            await Clients.GroupExcept(groupName,Context.ConnectionId).SendAsync(message);
        }

        public async Task SendGroupMessageDto(string groupName, MessageCreatedDto message)
        {
            await Clients.Group(groupName).SendAsync("ReceiveMessage", message);
        }

        public async Task SendBroadcastMessage(string message)
        {

            await Clients.All.SendAsync("ReceiveMessage", message);
        }

        public async Task SendBroadcastMessageDto(MessageCreatedDto message)
        {

            await Clients.All.SendAsync("ReceiveMessage", message);
        }



        public async Task LeaveGroup(string groupName)
        {

            await Groups.RemoveFromGroupAsync(Context.ConnectionId, groupName);
            
        }


        //After booking is accepted, we want the technician and the client both to join the group.

        public async Task JoinGroup(string groupName)
        {
           // await _chatService.JoinGroup(Context.ConnectionId, groupName);
         //   await Groups.AddToGroupAsync(Context.ConnectionId, groupName);
            var connId = Context.ConnectionId;
           // _userGroups.TryAdd(connId, new HashSet<string>());
            if(_userGroups.TryAdd(connId, new HashSet<string>()) == false) { throw new Exception("AlreadyExists"); }
             _userGroups.TryAdd(connId, new HashSet<string>());


            if (_userGroups[connId].Contains(groupName))
                return; // already in group, skip

            _userGroups[connId].Add(groupName);
            await Groups.AddToGroupAsync(connId, groupName);
        }

        

        private bool ValidateJwtToken(string token)
        {
            try
            {
                var key = Encoding.UTF8.GetBytes(_configuration["Jwt:Key"]!);

                var validationParameters = new TokenValidationParameters
                {
                    ValidateIssuer = true,
                    ValidateAudience = true,
                    ValidateLifetime = true,
                    ValidateIssuerSigningKey = true,
                    ValidIssuer = _configuration["Jwt:Issuer"],
                    ValidAudience = _configuration["Jwt:Audience"],
                    IssuerSigningKey = new SymmetricSecurityKey(key),
                    ClockSkew = TimeSpan.Zero
                };

                var principal = new JwtSecurityTokenHandler()
                    .ValidateToken(token, validationParameters, out _);

                return principal.Identity?.IsAuthenticated ?? false;
            }
            catch
            {
                return false;
            }


        }
    }
}
