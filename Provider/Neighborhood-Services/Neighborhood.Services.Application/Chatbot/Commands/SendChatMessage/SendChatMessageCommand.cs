using MediatR;
using Neighborhood.Services.Application.Chatbot.DTOs;

namespace Neighborhood.Services.Application.Chatbot.Commands.SendChatMessage
{
    public class SendChatMessageCommand : IRequest<ChatReplyDto>
    {
        // null = start a new session; otherwise continue the existing one
        public int? SessionId { get; set; }

        // The user's message
        public string Message { get; set; } = string.Empty;
    }
}
