using Neighborhood.Services.Domain.Shared;

namespace Neighborhood.Services.Domain.Disputes;

public class Dispute :BaseEntity<int>
{
    

    public int BookingId { get; set; }

    public int RaisedBy { get;  set; }

    public int? ResolvedByStaffId { get;  set; }

    public DisputeType DisputeType { get;  set; }

    public string Reason { get;  set; }

    public string? Resolution { get;  set; }

    public DisputeStatus Status { get;  set; }

    public DateTime CreatedAt { get;  set; }

    public DateTime? ResolvedAt { get; set; }


    // Empty Constructor For EF
    public Dispute()
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