using Microsoft.AspNetCore.SignalR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Neighborhood.Services.Domain.ApplicationUsers;
using Newtonsoft.Json.Linq;
using System.Collections.Concurrent;
using System.Security.Claims;
using System.Text.RegularExpressions;

namespace Neighborhood.Services.Infrastructure.Services.NotificationService
{
    public class NotificationHub : Hub
    {
        private readonly ILogger<NotificationHub> _logger;
public NotificationHub(ILogger<NotificationHub> logger)
        {
            _logger = logger;
            _logger.LogInformation("NotificationHub created");

        }
        // public string my_txt { set; get; }
        public ConcurrentDictionary<string, string> Connections { get; set; }
                = new(); 
        public override async Task OnConnectedAsync()
        {
            _logger.LogInformation(
        "Connected: {ConnectionId}",
        Context.ConnectionId);

            //using the role in identity
            var userRole = Context.GetHttpContext()?.User?.Claims.FirstOrDefault(c => c.Type == ClaimTypes.Role)?.Value;
           
            _logger.LogInformation($"\n\nuser role: {userRole}");
            Connections.TryAdd(Context.ConnectionId, userRole);
            _logger.LogInformation("Hello from hub!");
            _logger.LogDebug("hi");
            _logger.LogDebug($"Count:{Connections.Count}");

            _logger.LogInformation($"Count:{Connections.Count}");
            foreach (var connection in Connections)
            {
                _logger.LogInformation(
                    "ConnectionId: {ConnectionId}, UserId: {UserId}",
                    connection.Key,
                    connection.Value);
            }


          ;

            //using the enum role
           // var ur = Context.GetHttpContext()?.User?.Claims.FirstOrDefault(c => Enum.IsDefined(typeof(ApplicationUserRole), c))?.Value;
           // _logger.LogInformation($"\n\nuser role2: {ur}");

            var businessUserId = Context.GetHttpContext()?.User?.Claims.FirstOrDefault(c => c.Type == "NameIdentifier")?.Value;
            _logger.LogInformation($"\n\nbusiness: {businessUserId}");


            if (!string.IsNullOrEmpty(userRole))
                await Groups.AddToGroupAsync(Context.ConnectionId, userRole);

            //if (!string.IsNullOrEmpty(ur))
            //    await Groups.AddToGroupAsync(Context.ConnectionId, ur);


            if (!string.IsNullOrEmpty(businessUserId))
                await Groups.AddToGroupAsync(Context.ConnectionId, $"business-{businessUserId}");

            await base.OnConnectedAsync();
        }

        public override async Task OnDisconnectedAsync(Exception? exception)
        {
            await base.OnDisconnectedAsync(exception);
            //يعمل لليوزر ريموف من الجروب؟
        }
    }
}

