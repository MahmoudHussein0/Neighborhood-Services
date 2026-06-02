using Neighborhood.Services.Domain.Shared;

namespace Neighborhood.Services.Domain.SupportTickets
{


    public class SupportMessage : BaseEntity<int>
    {


        public int TicketId { get; set; }

        public int SenderId { get; set; }

        public string Message { get; set; }

        public MessageChannel Channel { get; set; }

        public DateTime? ReadAt { get; set; }

        public DateTime CreatedAt { get; set; }


        // Navigation Property
        public SupportTicket Ticket { get; set; }


        // Empty Constructor For EF
        public SupportMessage()
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
