using Neighborhood.Services.Domain.Shared;

namespace Neighborhood.Services.Domain.Chatbot
{
    public class ChatbotMessage : BaseEntity<int>
    {
        public int ChatbotSessionId { get; set; }

        // Who sent it: the User or the AI Assistant
        public ChatbotRole Role { get; set; }

        public string Content { get; set; } = string.Empty;

        public DateTime CreatedAt { get; set; }

        // Nav
        public ChatbotSession ChatbotSession { get; set; } = null!;
    }
}
