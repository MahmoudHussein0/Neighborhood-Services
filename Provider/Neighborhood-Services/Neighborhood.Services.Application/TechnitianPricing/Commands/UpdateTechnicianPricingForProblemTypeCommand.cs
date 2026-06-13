using MediatR;
using Neighborhood.Services.Application.TechnitianPricing.DTOs;


namespace Neighborhood.Services.Application.TechnitianPricing.Commands
{
    public class UpdateTechnicianPricingForProblemTypeCommand : IRequest<UpdatePricingDTO>
    {
        public int Id { get; set; }
        public decimal TechMinPrice { get; set; }
        public decimal TechMaxPrice { get; set; }

    }
}
