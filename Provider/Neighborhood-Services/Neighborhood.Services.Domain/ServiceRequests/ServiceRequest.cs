using System;
using System.Collections.Generic;
using System.Text;

namespace Neighborhood.Services.Domain.ServiceRequests
{
    public class ServiceRequest
    {
        // ----------------- Self Prop
        public int Id { get; set; }
        public string Description { get; set; } = string.Empty;
        public string Address { get; set; } = string.Empty;
        public string? Image { get; set; }
        public decimal Budget { get; set; }
        public ServiceRequestStatus Status { get; set; }
        public bool IsDeleted { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime ExpiresAt { get; set; }

        //------------ Foreign Keys
        public int CustomerId { get; set; }
        public int CategoryId { get; set; }
        public int ProblemTypeId { get; set; }

    }
}
