using MediatR;
using System;
using System.Collections.Generic;
using System.Text;

namespace Neighborhood.Services.Application.TechnitianPricing.Commands
{
    public class AddTechnicianPricingForProblemTypeCommand : IRequest<int>
    {
        public int TechnicianId { get; set; }
        public int ProblemTypeId { get; set; }
        public decimal MinPrice { get; set; }
        public decimal MaxPrice { get; set; }
    }
}
