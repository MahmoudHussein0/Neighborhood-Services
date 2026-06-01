using System;
using System.Collections.Generic;
using System.Text;

namespace Neighborhood.Services.Application.Conversations.DTOs
{
    public class ConversationSelectedDto
    {
        public int Id { get; set; }
        public int BookingId { get; set; }
        public string scssmsg { get; } = "Conversation Selected";
        public string LastMessage { set; get;}
    }
}
