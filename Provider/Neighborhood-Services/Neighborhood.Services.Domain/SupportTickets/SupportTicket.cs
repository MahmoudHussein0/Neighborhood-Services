using Neighborhood.Services.Domain.Shared;
using Neighborhood.Services.Domain.SupportTickets;

namespace Neighborhood.Services.Domain.SupportTickets;

public class SupportTicket: BaseEntity<int> 
{
   public int UserId { get;  set; }

    public int? BookingId { get;  set; }

    public string Subject { get;  set; }

    public SupportTicketStatus Status { get; set; }

    public DateTime CreatedAt { get;set; }

    public DateTime UpdatedAt { get; set; }


    // Navigation Property
    public ICollection<SupportMessage> Messages { get;set; }


    // Empty Constructor For EF
    private SupportTicket()
    {
    }


    // Main Constructor
    public SupportTicket(
        int userId,
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