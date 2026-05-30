using Neighborhood.Services.Domain.ApplicationUsers;
using Neighborhood.Services.Domain.Shared;

namespace Neighborhood.Services.Domain.Message
{
    public class Message : BaseEntity<int>
    {
        public int ConversationId { get; set; }
        public string SenderId { get; set; }
        public string content { get; set; }
        public bool isRead { get; set; }

        public DateTime createdAt {  get; set; }


        public ApplicationUser Sender { set; get; } = null;
        public Conversation.Conversation Conversation { set; get; } =null;
    }
}
