using MediatR;
using System;
using System.Collections.Generic;
using System.Text;

namespace Neighborhood.Services.Application.TechnitianPricing.Commands
{
    public class RemoveTechnicianPricingForProblemTypeCommand : IRequest<bool>
    {
        public int  Id { get; set; }
    }
}
