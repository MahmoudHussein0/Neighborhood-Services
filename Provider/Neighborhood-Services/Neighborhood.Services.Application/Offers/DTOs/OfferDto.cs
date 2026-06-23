using Neighborhood.Services.Domain.Offers;

namespace Neighborhood.Services.Application.Offers.DTOs
{
    public class OfferDto
    {
        public int Id { get; set; }
        public int ServiceRequestId { get; set; }
        public int TechnicianId { get; set; }
        // The customer who posted the service request this offer is on — lets the technician's
        // Offers page link through to the customer's public profile (shown by name).
        public int CustomerId { get; set; }
        public string CustomerName { get; set; } = string.Empty;
        // A brief of the request the offer is on, so the tech can see what they bid on.
        public string ServiceRequestDescription { get; set; } = string.Empty;
        public string ServiceRequestAddress { get; set; } = string.Empty;
        public decimal Price { get; set; }
        public int EstimatedDuration { get; set; }
        public string Message { get; set; } = string.Empty;
        public DateTime ScheduledAt { get; set; }
        public OfferStatus Status { get; set; }
        public DateTime CreatedAt { get; set; }
    }
}
