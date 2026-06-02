using MediatR;
using Neighborhood.Services.Application.TechnitianPricing.DTOs;


namespace Neighborhood.Services.Application.TechnitianPricing.Commands
{
    public class UpdateTechnicianPricingForProblemTypeCommand : IRequest<UpdatePricingDTO>
    {
        public int Id { get; set; }
        public decimal MinPrice { get; set; }
        public decimal MaxPrice { get; set; }

    }
}
