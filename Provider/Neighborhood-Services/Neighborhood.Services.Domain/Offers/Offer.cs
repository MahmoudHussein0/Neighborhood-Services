using System;
using System.Collections.Generic;
using System.Text;

namespace Neighborhood.Services.Domain.Offers
{
    public class Offer
    {
        //------- Self Prop
        public int Id { get; set; }
        public decimal Price { get; set; }
        public int EstimatedDuration { get; set; }
        public string Message { get; set; } = string.Empty;
        public OfferStatus Status { get; set; }
        public bool IsDeleted { get; set; }
        public DateTime CreatedAt { get; set; }

        //----- Foreign Keys
        public int ServiceRequestId { get; set; }
        public int TechnicianId { get; set; }
    }
}
