using MediatR;
using Neighborhood.Services.Application.TechnitianPricing.DTOs;


namespace Neighborhood.Services.Application.TechnitianPricing.Queries
{
    public  class GetPricingForProblemTypeQuery  : IRequest<IReadOnlyList<TechnicianPricingDto>>
    {

        public string Lang  { get; set; }

        public int  TechnicianId { get; set; }
        public GetPricingForProblemTypeQuery(int technicianId)
        {
            TechnicianId = technicianId;
        }
    }
}
