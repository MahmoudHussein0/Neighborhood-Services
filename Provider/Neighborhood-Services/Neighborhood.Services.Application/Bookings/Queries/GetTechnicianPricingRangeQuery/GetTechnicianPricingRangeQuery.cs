using MediatR;
using Neighborhood.Services.Application.Bookings.DTOs;

namespace Neighborhood.Services.Application.Bookings.Queries.GetTechnicianPricingRangeQuery
{
    // Returns the technician's MinPrice/MaxPrice for a single problem type, or null
    // if the tech hasn't priced this problem type yet. Used by the booking flow:
    //  * customer Book Now modal — to display "Tech's range for this: X–Y EGP"
    //  * technician Quote modal — to display + enforce the input range
    public class GetTechnicianPricingRangeQuery : IRequest<TechnicianPricingRangeDto?>
    {
        public int TechnicianId { get; set; }
        public int ProblemTypeId { get; set; }
    }
}
