namespace Neighborhood.Services.Domain.Disputes;

public class Dispute
{
    public int Id { get; private set; }

    public int BookingId { get; private set; }

    public int RaisedBy { get; private set; }

    public int? ResolvedByStaffId { get; private set; }

    public DisputeType DisputeType { get; private set; }

    public string Reason { get; private set; }

    public string? Resolution { get; private set; }

    public DisputeStatus Status { get; private set; }

    public DateTime CreatedAt { get; private set; }

    public DateTime? ResolvedAt { get; private set; }


    // Empty Constructor For EF
    private Dispute()
    {
    }


    // Main Constructor
    public Dispute(
        int bookingId,
        int raisedBy,
        DisputeType disputeType,
        string reason)
    {
        BookingId = bookingId;
        RaisedBy = raisedBy;
        DisputeType = disputeType;
        Reason = reason;

        Status = DisputeStatus.Open;

        CreatedAt = DateTime.UtcNow;
    }


    //// Business Method
    //public void Resolve(
    //    int staffId,
    //    string resolution)
    //{
    //    ResolvedByStaffId = staffId;

    //    Resolution = resolution;

    //    Status = DisputeStatus.Resolved;

    //    ResolvedAt = DateTime.UtcNow;
    //}


    //public void StartReview()
    //{
    //    Status = DisputeStatus.UnderReview;
    //}
}