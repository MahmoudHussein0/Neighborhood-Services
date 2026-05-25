using Neighborhood.Services.Domain.Bookings;
using Neighborhood.Services.Domain.Categories;
using Neighborhood.Services.Domain.Customers;
using Neighborhood.Services.Domain.Offers;
using Neighborhood.Services.Domain.ProblemTypes;
using Neighborhood.Services.Domain.Shared;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Text;

namespace Neighborhood.Services.Domain.ServiceRequests
{
    public class ServiceRequest : BaseEntity<int>
    {
        // ----------------- Self Prop
        public string Description { get; set; } = string.Empty;
        public string Address { get; set; } = string.Empty;
        public string? Image { get; set; }
        public decimal Budget { get; set; }
        public ServiceRequestStatus Status { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime ExpiresAt { get; set; }
        public Point Location { get; set; }

        //------------ Foreign Keys
        public int CustomerId { get; set; }
        public int CategoryId { get; set; }
        public int ProblemTypeId { get; set; }


        // Nav
        public Customer Customer { get; set; } = null!;
        public Category Category { get; set; } = null!;
        public ProblemType ProblemType { get; set; } = null!;
        public Booking? Booking { get; set; }
        public ICollection<Offer> Offers { get; set; } = new HashSet<Offer>();


    }
}
