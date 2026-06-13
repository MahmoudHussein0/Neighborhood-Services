using MediatR;
using System;
using System.Collections.Generic;
using System.Text;

namespace Neighborhood.Services.Application.TechnitianPricing.Commands
{
    public class AddTechnicianPricingForProblemTypeCommand : IRequest<int>
    {
        public int ProblemTypeId { get; set; }
        public decimal TechMinPrice { get; set; }
        public decimal TechMaxPrice { get; set; }
    }
}
