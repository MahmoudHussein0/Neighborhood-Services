namespace Neighborhood.Services.Application.Chatbot.DTOs
{
    public class ChatReplyDto
    {
        // The session id — returned so the frontend knows which session to continue
        public int SessionId { get; set; }

        // The assistant's reply text
        public string Reply { get; set; } = string.Empty;
    }
}
