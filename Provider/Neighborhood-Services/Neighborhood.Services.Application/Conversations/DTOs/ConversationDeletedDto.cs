using System;
using System.Collections.Generic;
using System.Text;

namespace Neighborhood.Services.Application.Conversations.DTOs
{
    public class ConversationDeletedDto
    {
        public int BookingId { get; set; }
        public string ClientId { get; set; }
        public string TechnicianId { get; set; }

        public DateTime CreatedAt { get; set; }
        public DateTime DeletedAt { get; set; }

        public string scssmsg { get; } = "Conversation Deleted";
    }
}
