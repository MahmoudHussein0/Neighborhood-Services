using Neighborhood.Services.Domain.Bookings;
using Neighborhood.Services.Domain.ServiceRequests;
using Neighborhood.Services.Domain.Shared;
using Neighborhood.Services.Domain.Technicians;
using System;
using System.Collections.Generic;
using System.Text;

namespace Neighborhood.Services.Domain.Offers
{
    public class Offer :BaseEntity<int>
    {
        //------- Self Prop
        public decimal Price { get; set; }
        public int EstimatedDuration { get; set; }
        public string Message { get; set; } = string.Empty;
        public OfferStatus Status { get; set; }
        // Technician's proposed service time (may differ from the customer's desired time)
        public DateTime ScheduledAt { get; set; }
        public DateTime CreatedAt { get; set; }

        //----- Foreign Keys
        public int ServiceRequestId { get; set; }
        public int TechnicianId { get; set; }

        //Nav
        public ServiceRequest ServiceRequest { get; set; } = null!;
        public Technician Technician { get; set; } = null!;
        public Booking? Booking { get; set; }
    }
}
