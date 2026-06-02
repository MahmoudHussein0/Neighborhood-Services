using Microsoft.AspNetCore.SignalR;
using Microsoft.EntityFrameworkCore;
using Neighborhood.Services.Domain.ApplicationUsers;
using Newtonsoft.Json.Linq;
using System.Security.Claims;
using System.Text.RegularExpressions;

namespace Neighborhood.Services.Infrastructure.Services.NotificationService
{
    public class NotificationHub : Hub
    {
        public override async Task OnConnectedAsync()
        {
            //using the role in identity
            var userRole = Context.GetHttpContext()?.User?.Claims.FirstOrDefault(c => c.Type == ClaimTypes.Role)?.Value;
            //using the enum role
            var ur = Context.GetHttpContext()?.User?.Claims.FirstOrDefault(c => Enum.IsDefined(typeof(ApplicationUserRole), c))?.Value;

            var businessUserId = Context.GetHttpContext()?.User?.Claims.FirstOrDefault(c => c.Type == "Id")?.Value;


            if (!string.IsNullOrEmpty(userRole))
                await Groups.AddToGroupAsync(Context.ConnectionId, userRole);

            if (!string.IsNullOrEmpty(ur))
                await Groups.AddToGroupAsync(Context.ConnectionId, ur);


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

