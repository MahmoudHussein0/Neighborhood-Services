using System;
using System.Collections.Generic;
using System.Text;

namespace Neighborhood.Services.Application.Conversations.DTOs
{
    public class ConversationUpdatedDto
    {
        public int BookingId { get; set; }
        public string Message { get; set; }

        public string MessageSenderId { get; set; }
        public string MessageSenderName { get; set; }

        public string LastMessage { get; set; }

        public DateTime UpdatedAt { get; set; }
    }
}
