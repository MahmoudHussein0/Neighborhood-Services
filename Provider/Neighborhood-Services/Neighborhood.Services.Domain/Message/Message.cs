using Neighborhood.Services.Domain.ApplicationUsers;
using Neighborhood.Services.Domain.Shared;

namespace Neighborhood.Services.Domain.Message
{
    public class Message : BaseEntity<int>
    {
        public int ConversationId { get; set; }
        public int SenderId { get; set; }
        public string content { get; set; }
        public bool isRead { get; set; }

        public DateTime createdAt = DateTime.UtcNow;

        public ApplicationUser Sender { set; get; } = new ApplicationUser();
        public Conversation.Conversation Conversation { set; get; } = new Conversation.Conversation();
    }
}
