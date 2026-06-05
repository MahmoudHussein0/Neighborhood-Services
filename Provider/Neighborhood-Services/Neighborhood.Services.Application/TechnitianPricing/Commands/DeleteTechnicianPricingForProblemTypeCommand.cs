using MediatR;
using System;
using System.Collections.Generic;
using System.Text;

namespace Neighborhood.Services.Application.TechnitianPricing.Commands
{
    public class DeleteTechnicianPricingForProblemTypeCommand : IRequest<bool>
    {
    
        public int  Id { get; set; }
        public DeleteTechnicianPricingForProblemTypeCommand(int id)
        {
            Id = id;
        }

    }
}
