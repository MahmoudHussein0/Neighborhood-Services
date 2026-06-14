using System;
using System.Collections.Generic;
using System.Text;

namespace Neighborhood.Services.Application.Messages.DTOs
{
    public class MessageSelectedDto
    {
        public int conversationId { set; get; }
        public int messageId { set; get;  }
        public string content { set; get; }

        public string senderId { set; get;  }
        public string senderName { set; get; }
        public bool read {  set; get; }
        public bool deleted { set; get; }

        public DateTime sentAt { set; get; } = DateTime.UtcNow;

    }
}
