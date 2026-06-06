namespace Neighborhood.Services.Application.Chatbot.DTOs
{
    // Summary of a session — for listing a user's past chats
    public class ChatSessionDto
    {
        public int Id { get; set; }
        public string? Title { get; set; }
        public DateTime CreatedAt { get; set; }
    }

    // One message in a session
    public class ChatMessageDto
    {
        public string Role { get; set; } = string.Empty;   // "User" or "Assistant"
        public string Content { get; set; } = string.Empty;
        public DateTime CreatedAt { get; set; }
    }

    // A session with its full message history
    public class ChatSessionDetailDto
    {
        public int Id { get; set; }
        public string? Title { get; set; }
        public DateTime CreatedAt { get; set; }
        public List<ChatMessageDto> Messages { get; set; } = new();
    }
}
