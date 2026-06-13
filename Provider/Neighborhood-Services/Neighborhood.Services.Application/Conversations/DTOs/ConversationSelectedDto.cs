using System;
using System.Collections.Generic;
using System.Text;

namespace Neighborhood.Services.Application.Conversations.DTOs
{
    public class ConversationSelectedDto
    {
        public int id { get; set; }
        public int bookingId { get; set; }
        public string scssmsg { get; } = "Conversation Selected";
        public string lastMessage { set; get;}

        public string othersName { set; get; }
        public string coversationImage { set; get; }

        public string bookingDescription { set; get; }
        public string messageSenderId { get; set; }
        public string messageSenderName { get; set; }
        public DateTime updatedAt { get; set; }
    }
}
