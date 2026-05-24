using System;
using System.Collections.Generic;
using System.Text;
using Neighborhood.Services.Domain.Shared;


namespace Neighborhood.Services.Domain.Message
{
    public class Message:BaseEntity<int>
    {
       //foriegn Key
        public int ConversationId { get; set; }
        //foriegn Key
        public int SenderId { get; set; }
        public string content { get; set; }
        public bool isRead { get; set; }

        public DateTime createdAt = DateTime.UtcNow;
    }
}
