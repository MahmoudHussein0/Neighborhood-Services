using Neighborhood.Services.Domain.ApplicationUsers;
using Neighborhood.Services.Domain.Bookings;
using Neighborhood.Services.Domain.PromoCodes;
using Neighborhood.Services.Domain.Shared;

namespace Neighborhood.Services.Domain.SupportTickets;

public class SupportTicket : BaseEntity<int>
{
    public string UserId { get; set; }

    public int? BookingId { get; set; }

    public string Subject { get; set; }
    public string Description { get; set; }
    public SupportTicketStatus Status { get; set; }
    public SupportTicketPriority Priority { get; set; } = SupportTicketPriority.Low;
    public DateTime CreatedAt { get; set; }

    public DateTime UpdatedAt { get; set; }

    public int? PromoCodeUsageId { get; set; }



    // Navigation Property
    public ICollection<SupportMessage> Messages { get; set; }

    public ApplicationUser User { get; set; }

    public Booking? Booking { get; set; }
    public PromoCodeUsage? PromoCodeUsage { get; set; }

    // Empty Constructor For EF
    public SupportTicket()
    {
    }

}