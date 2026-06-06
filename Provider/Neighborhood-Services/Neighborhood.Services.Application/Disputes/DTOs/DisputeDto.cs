namespace Neighborhood.Services.Application.Disputes.DTOs
{
    public class DisputeDto
    {
        public int Id { get; set; }
        public int BookingId { get; set; }
        public int RaisedBy { get; set; }
        public int? ResolvedByStaffId { get; set; }
        public string DisputeType { get; set; }
        public string Reason { get; set; }
        public string? Resolution { get; set; }
        public string Status { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime? ResolvedAt { get; set; }
    }
}
