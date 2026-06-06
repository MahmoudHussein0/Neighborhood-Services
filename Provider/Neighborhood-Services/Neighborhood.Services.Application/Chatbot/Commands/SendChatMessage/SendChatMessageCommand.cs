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


        // Optional: user's region (city), e.g. "Alexandria" / "Cairo".
        // Sent by the frontend from the logged-in user's saved address. Null for guests.
        // Used by the price estimation service to give a localized estimate.
        public string? Region { get; set; }

    }
}
