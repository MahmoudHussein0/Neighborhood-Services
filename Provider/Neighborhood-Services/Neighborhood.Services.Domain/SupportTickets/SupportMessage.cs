using Neighborhood.Services.Domain.ApplicationUsers;
using Neighborhood.Services.Domain.Shared;

namespace Neighborhood.Services.Domain.SupportTickets
{


    public class SupportMessage : BaseEntity<int>
    {


        public int TicketId { get; set; }

        public string? SenderId { get; set; }

        public string? Message { get; set; }

        public MessageChannel Channel { get; set; }

        public DateTime? ReadAt { get; set; }

        public DateTime CreatedAt { get; set; }


        // Navigation Property
        public SupportTicket Ticket { get; set; } = null!;

        public ApplicationUser Sender { get; set; } = null!;

        public ICollection<MessageAttachment>? Attachments { get; set; }
    = new List<MessageAttachment>();
        public MessageSenderType SenderType { get; set; }

        // Empty Constructor For EF
        public SupportMessage()
        {
        }


   
       



    }
}
