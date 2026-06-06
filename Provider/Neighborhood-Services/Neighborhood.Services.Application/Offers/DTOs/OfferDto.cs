using Neighborhood.Services.Domain.Offers;

namespace Neighborhood.Services.Application.Offers.DTOs
{
    public class OfferDto
    {
        public int Id { get; set; }
        public int ServiceRequestId { get; set; }
        public int TechnicianId { get; set; }
        public decimal Price { get; set; }
        public int EstimatedDuration { get; set; }
        public string Message { get; set; } = string.Empty;
        public DateTime ScheduledAt { get; set; }
        public OfferStatus Status { get; set; }
        public DateTime CreatedAt { get; set; }
    }
}
