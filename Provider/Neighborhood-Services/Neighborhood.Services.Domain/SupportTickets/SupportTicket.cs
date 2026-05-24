using Neighborhood.Services.Domain.SupportTickets;

namespace Neighborhood.Services.Domain.SupportTickets;

public class SupportTicket
{
    public int Id { get; private set; }

    public int UserId { get; private set; }

    public int? BookingId { get; private set; }

    public string Subject { get; private set; }

    public SupportTicketStatus Status { get; private set; }

    public bool IsDeleted { get; private set; }

    public DateTime CreatedAt { get; private set; }

    public DateTime UpdatedAt { get; private set; }


    // Navigation Property
    public ICollection<SupportMessage> Messages { get; private set; }


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


    public void MarkInProgress()
    {
        Status = SupportTicketStatus.InProgress;

        UpdatedAt = DateTime.UtcNow;
    }


    public void Resolve()
    {
        Status = SupportTicketStatus.Resolved;

        UpdatedAt = DateTime.UtcNow;
    }


    public void WaitingCustomer()
    {
        Status = SupportTicketStatus.WaitingOnCustomer;

        UpdatedAt = DateTime.UtcNow;
    }


    public void Delete()
    {
        IsDeleted = true;

        UpdatedAt = DateTime.UtcNow;
    }
}