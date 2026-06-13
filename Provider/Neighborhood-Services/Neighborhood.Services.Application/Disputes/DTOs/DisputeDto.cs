namespace Neighborhood.Services.Application.Disputes.DTOs
{
    public class DisputeDto
    {
        public int Id { get; set; }
        public int BookingId { get; set; }
        public string RaisedByUserId { get; set; }

        public int? ResolvedByStaffId { get; set; }
        public string DisputeType { get; set; }
        public string Reason { get; set; }
        public string? Resolution { get; set; }
        public string Status { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime? ResolvedAt { get; set; }

        // ── Enrichment (populated by GetDetailByIdAsync for the staff details modal) ──
        // The two parties to the disputed booking, so staff can ban either one.
        public string? CustomerUserId { get; set; }
        public string? TechnicianUserId { get; set; }
        public string? CustomerName { get; set; }
        public string? TechnicianName { get; set; }

        // The booking's escrow, so staff can refund the customer or release to the technician.
        public int? EscrowId { get; set; }
        public string? EscrowStatus { get; set; }
        public decimal? EscrowAmount { get; set; }
    }
}
