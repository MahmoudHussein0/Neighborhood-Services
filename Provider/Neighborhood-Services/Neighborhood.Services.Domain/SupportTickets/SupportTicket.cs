using Neighborhood.Services.Domain.ApplicationUsers;
using Neighborhood.Services.Domain.Bookings;
using Neighborhood.Services.Domain.PromoCodes;
using Neighborhood.Services.Domain.Shared;
using Neighborhood.Services.Domain.SupportTickets;

public class SupportTicket : BaseEntity<int>
{
    public string? UserId { get; set; }

    public string SenderName { get; set; }
    public string SenderEmail { get; set; }

    public int? BookingId { get; set; }

    public string Subject { get; set; }
    public string Description { get; set; }

    public SupportTicketStatus Status { get; set; }
    public SupportTicketPriority Priority { get; set; } = SupportTicketPriority.Low;

    public DateTime CreatedAt { get; set; }
    public DateTime UpdatedAt { get; set; }

    public int? PromoCodeUsageId { get; set; }

    public ICollection<SupportMessage> Messages { get; set; }

    public ApplicationUser? User { get; set; }

    public Booking? Booking { get; set; }
    public PromoCodeUsage? PromoCodeUsage { get; set; }

    public SupportTicket()
    {
    }
}