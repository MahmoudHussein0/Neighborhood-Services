using Neighborhood.Services.Domain.Shared;

namespace Neighborhood.Services.Domain.SupportTickets;

public class SupportTicket : BaseEntity<int>
{
    public string UserId { get; set; }

    public int? BookingId { get; set; }

    public string Subject { get; set; }

    public SupportTicketStatus Status { get; set; }

    public DateTime CreatedAt { get; set; }

    public DateTime UpdatedAt { get; set; }


    // Navigation Property
    public ICollection<SupportMessage> Messages { get; set; }


    // Empty Constructor For EF
    public SupportTicket()
    {
    }


    // Main Constructor
    public SupportTicket(
        string userId,
        int? bookingId,
        string subject)
    {
        UserId = userId;

        BookingId = bookingId;

        Subject = subject;

        Status = SupportTicketStatus.Open;

        CreatedAt = DateTime.UtcNow;

        UpdatedAt = DateTime.UtcNow;

        IsDeleted = false;
    }



}