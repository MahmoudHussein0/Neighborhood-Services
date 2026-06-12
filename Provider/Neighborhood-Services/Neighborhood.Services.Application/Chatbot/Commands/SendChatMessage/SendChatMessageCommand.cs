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
    }
}
