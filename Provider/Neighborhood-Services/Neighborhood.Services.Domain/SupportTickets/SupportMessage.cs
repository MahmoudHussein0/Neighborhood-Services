using System;
using System.Collections.Generic;
using System.Text;

namespace Neighborhood.Services.Domain.SupportTickets
{
    

    public class SupportMessage
    {
        public int Id { get; private set; }

        public int TicketId { get; private set; }

        public int SenderId { get; private set; }

        public string Message { get; private set; }

        public MessageChannel Channel { get; private set; }

        public bool IsDeleted { get; private set; }

        public DateTime? ReadAt { get; private set; }

        public DateTime CreatedAt { get; private set; }


        // Navigation Property
        public SupportTicket Ticket { get; private set; }


        // Empty Constructor For EF
        private SupportMessage()
        {
        }


        // Main Constructor
        public SupportMessage(
            int ticketId,
            int senderId,
            string message,
            MessageChannel channel)
        {
            TicketId = ticketId;

            SenderId = senderId;

            Message = message;

            Channel = channel;

            CreatedAt = DateTime.UtcNow;

            IsDeleted = false;
        }


   
    }
}
