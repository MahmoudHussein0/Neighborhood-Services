using Neighborhood.Services.Domain.Offers;
using System;
using System.Collections.Generic;
using System.Text;

namespace Neighborhood.Services.Application.ServiceRequests.DTOs
{
    public class OfferSummaryDto
    {
        public int Id { get; set; }
        public decimal Price { get; set; }
        public int EstimatedDuration { get; set; }
        public string Message { get; set; } = string.Empty;
        public OfferStatus Status { get; set; }
        public int TechnicianId { get; set; }
        public string TechnicianName { get; set; } = string.Empty;
        public double TechnicianRating { get; set; }
        public DateTime CreatedAt { get; set; }
    }
}
