using MediatR;
using Microsoft.AspNetCore.SignalR;
using Neighborhood.Services.Application.Disputes.Commands;
using Neighborhood.Services.Application.Notifications.Services;
using Neighborhood.Services.Application.SupportTickets.Commands;
using Neighborhood.Services.Domain.ApplicationUsers;
using Neighborhood.Services.Domain.Notifications;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Security.Claims;
using System.Text;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory.Database;

namespace Neighborhood.Services.Infrastructure.Services.CustomerService
{
    public class ticketDto
    {
        public string subject { get; set; }
        public string description { get; set; }
        public string senderName { get; set; }
        public string senderEmail { get; set; }
    }
    public class CustomerServiceHub:Hub
    {

        private readonly IMediator _mediator;
        private readonly INotificationService _service;

        public CustomerServiceHub(IMediator mediator,INotificationService service)
        {
            _mediator = mediator;
            _service= service;
        }
        private static readonly ConcurrentDictionary<string, HashSet<string>> _userGroups = new();

        public override async Task OnConnectedAsync()
        {
            //Adding admins to a group to listen to the hub
            var userRole = Context.GetHttpContext()?.User?.Claims.FirstOrDefault(c => c.Type == ClaimTypes.Role)?.Value.ToString();
            if (!string.IsNullOrEmpty(userRole))
                await Groups.AddToGroupAsync(Context.ConnectionId, userRole);

            await base.OnConnectedAsync();
        }
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
                    await SendGroupMessage(group, "Server Failed! Try to reconnect again", "system");
                }
            }

            await base.OnDisconnectedAsync(exception);
        }

        //هنبعتلها الإيميل
        public async Task JoinGroup(string groupName)
        {
           
            var connId = Context.ConnectionId;
            if (_userGroups.TryAdd(connId, new HashSet<string>()) == false) { throw new Exception("AlreadyExists"); }
            _userGroups.TryAdd(connId, new HashSet<string>());


            if (_userGroups[connId].Contains(groupName))
                return; // already in group so skipp

            _userGroups[connId].Add(groupName);
            await Groups.AddToGroupAsync(connId, groupName);
        }

        public async Task SendLiveTicket(string userEmail, ticketDto ticket)
        {
            ticket.senderEmail = userEmail;
            ticket.senderName = "guest";
          //  var result = await _mediator.Send(ticket);
            await JoinGroup(userEmail);
           
            await _service.SendDirectiveNotificationToUser("7d466805-6429-4940-9434-4990d80263b7", "There is a new Live Ticket",NotificationTypes.support);
            await Clients.Group(ApplicationUserRole.Staff.ToString()).SendAsync("ReceiveTicket",ticket);
        }
        //هيسند التيكت، هينضم للهاب، هيبعت ل

        public async Task SendGroupMessage(string emaill, string mssg, string sender) //sender:Admin or guest?
        {
            await Clients.Group(emaill).SendAsync("TicketChat", new { email = emaill, message = mssg, sndr=sender });
          //  await Clients.Group(ApplicationUserRole.Staff.ToString()).SendAsync("TicketChat", new { email = emaill, message = mssg });

        }

    }
}
