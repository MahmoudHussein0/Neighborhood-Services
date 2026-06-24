using Neighborhood.Services.Domain.ServiceRequests;

namespace Neighborhood.Services.Application.ServiceRequests.DTOs
{
    public class ServiceRequestSummaryDto
    {
        public int Id { get; set; }
        public string Description { get; set; } = string.Empty;
        public string Address { get; set; } = string.Empty;
        public decimal Budget { get; set; }
        public ServiceRequestStatus Status { get; set; }
        public DateTime ScheduledAt { get; set; }
        public int CustomerId { get; set; }
        public string CustomerName { get; set; } = string.Empty;
        public int ProblemTypeId { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime ExpiresAt { get; set; }
        public int OfferCount { get; set; }
        public double Latitude { get; set; }
        public double Longitude { get; set; }
        public int CategoryId { get; set; }

        // True when the viewing technician is assigned to this request's category
        // (i.e. they are allowed to make an offer). Only meaningful for the browse-open feed.
        public bool CanOffer { get; set; }
    }
}
