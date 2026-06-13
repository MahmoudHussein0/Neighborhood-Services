using MediatR;
using Neighborhood.Services.Application.TechnitianPricing.DTOs;


namespace Neighborhood.Services.Application.TechnitianPricing.Queries
{
    public  class GetPricingForProblemTypeQuery  : IRequest<IReadOnlyList<TechnicianPricingDto>>
    {

        public string Lang  { get; set; }

        public GetPricingForProblemTypeQuery(string lang)
        {
            Lang = lang;
        }
    }

}
