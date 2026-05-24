using MediatR;
using Microsoft.AspNetCore.SignalR;
using Microsoft.IdentityModel.Tokens;
using Neighborhood.Services.Application.Modules.Messages.Commands;
using System.IdentityModel.Tokens.Jwt;
using System.Text;
using System.Security.Claims;
using Jose;
using Neighborhood.Services.Application.Messages;
using Neighborhood.Services.Application.Messages.DTOs;

namespace Neighborhood.Services.API.Hubs
{
    public class ChatHub:Hub
    {

        private readonly IMessageRepository _messageRepo;
        private readonly IMediator _mediator;
        private readonly JwtOptions _jwtOptions;


        public ChatHub(IMessageRepository messageRepo, IMediator mediator)
        {
            _messageRepo = messageRepo;
            _mediator = mediator;
        }
        //public override async Task OnConnectedAsync()
        //{
        //    var token = Context.GetHttpContext()?.Request.Query["access_token"].ToString();

        //    if (string.IsNullOrEmpty(token))
        //    {
        //        await Clients.Caller.SendAsync("Error", "Authentication failed: No token provided");
        //        Context.Abort();
        //        return;
        //    }

        //    if (!ValidateJwtToken(token))
        //    {
        //        await Clients.Caller.SendAsync("Error", "Authentication failed: Invalid token");
        //        Context.Abort();
        //        return;
        //    }

        //    await base.OnConnectedAsync();
        //}

        //private bool ValidateJwtToken(string token)
        //{
        //    try
        //    {
        //        var tokenHandler = new JwtSecurityTokenHandler();
        //        var key = Encoding.UTF8.GetBytes("kkkkkkk");
        //        var validationParameters = new TokenValidationParameters
        //        {
        //            ValidateIssuer = true,
        //            ValidateAudience = true,
        //            ValidateLifetime = true,
        //            ValidIssuer = _jwtOptions.Issuer,
        //            ValidAudience = _jwtOptions.Audience,
        //            IssuerSigningKey = new SymmetricSecurityKey(key)
        //        };

        //        SecurityToken validatedToken;
        //        var principal = tokenHandler.ValidateToken(token, validationParameters, out validatedToken);

        //        return principal.Identity.IsAuthenticated;

        //    }
        //    catch (Exception)
        //    {
        //        return false;
        //    }
        //}
        public async Task JoinRoom(int roomId)
        {
            await Groups.AddToGroupAsync(Context.ConnectionId, roomId.ToString());
        }
        public async Task SendMessage( int roomId,string message)
        {
            var userId = Context.UserIdentifier;

            MessageCreatedDto savedMessage = await _mediator.Send(new CreateMessageCommand() {
            SenderId =int.Parse(userId) ,ConversationId=roomId,content=message});
                   

            await Clients.Group(roomId.ToString()).SendAsync("ReceiveMessage",savedMessage);
        }
    }
}

