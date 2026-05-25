using System;
using System.Collections.Generic;
using System.Text;
using Neighborhood.Services.Domain.Shared;
using Neighborhood.Services.Domain.ApplicationUser;
using Neighborhood.Services.Domain.Conversation;


namespace Neighborhood.Services.Domain.Message
{
    public class Message : BaseEntity<int>
    {
        //foriegn Key
        public int ConversationId { get; set; }
        //foriegn Key
        public int SenderId { get; set; }
        public string content { get; set; }
        public bool isRead { get; set; }

        public DateTime createdAt = DateTime.UtcNow;


        public ApplicationUser.ApplicationUser Sender { set; get; } = new ApplicationUser.ApplicationUser();
        public Conversation.Conversation Conversation { set; get; } =new Conversation.Conversation();
    }
}

    
