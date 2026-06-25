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

        // Optional: a Cloudinary image URL the user attached (e.g. a photo of the problem).
        // When present, it's sent to the vision model for THIS turn only (not replayed).
        public string? ImageUrl { get; set; }


        // Optional: user's region (city), e.g. "Alexandria" / "Cairo".
        // Explicit override/fallback. The handler also resolves the region from GPS
        // (below) or the message text, normalizing to one of the price service's keys.
        public string? Region { get; set; }

        // Optional GPS coordinates the frontend captures when the user taps "share location".
        // When present, the handler reverse-geocodes them and normalizes the resulting
        // address to a known region key for a localized price estimate.
        public double? Latitude { get; set; }
        public double? Longitude { get; set; }

        // The recent conversation turns BEFORE this message, sent by the frontend so the chat
        // has memory even for guests (who have no saved session). Excludes the current Message.
        // When empty, the handler falls back to a logged-in user's saved session messages.
        public List<ChatHistoryTurn> History { get; set; } = new();
    }

    // One prior conversation turn the frontend replays for context.
    public class ChatHistoryTurn
    {
        // "User" or "Assistant".
        public string Role { get; set; } = "User";
        public string Content { get; set; } = string.Empty;
    }
}
