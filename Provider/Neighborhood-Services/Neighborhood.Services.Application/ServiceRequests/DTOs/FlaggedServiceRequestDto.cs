namespace Neighborhood.Services.Application.ServiceRequests.DTOs
{
    // Shown in the staff moderation queue. Includes the Image so staff can eyeball
    // exactly what the agent flagged before approving or rejecting.
    public class FlaggedServiceRequestDto
    {
        public int Id { get; set; }
        public string Description { get; set; } = string.Empty;
        public string Address { get; set; } = string.Empty;
        public string? Image { get; set; }
        public decimal Budget { get; set; }
        public int CustomerId { get; set; }
        public DateTime ScheduledAt { get; set; }
        public DateTime CreatedAt { get; set; }
    }
}
