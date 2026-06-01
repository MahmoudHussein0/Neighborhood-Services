using System;
using System.Collections.Generic;
using System.Text;

namespace Neighborhood.Services.Application.Conversations.DTOs
{
    public class ConversationCreatedDto
    {
        public int BookingId { get; set; }
        public int ClientId { get; set; }
        public int TechnicianId { get; set; }

        public DateTime CreatedAt {  get; set; }

        public string scssmsg { get; } = "Conversation Created";
    }
}
