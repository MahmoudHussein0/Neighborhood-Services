using Neighborhood.Services.Domain.Shared;

namespace Neighborhood.Services.Domain.Chatbot
{
    public class ChatbotSession : BaseEntity<int>
    {
        // The identity user this conversation belongs to
        public string UserId { get; set; } = string.Empty;

        // A short label for the session (e.g. the first user message)
        public string? Title { get; set; }

        public DateTime CreatedAt { get; set; }

        // Nav
        public ICollection<ChatbotMessage> Messages { get; set; } = new List<ChatbotMessage>();
    }
}
