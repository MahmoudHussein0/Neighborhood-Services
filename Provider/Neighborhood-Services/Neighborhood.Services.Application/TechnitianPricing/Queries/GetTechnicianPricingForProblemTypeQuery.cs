using MediatR;
using Neighborhood.Services.Application.TechnitianPricing.DTOs;
using System;
using System.Collections.Generic;
using System.Text;

namespace Neighborhood.Services.Application.TechnitianPricing.Queries
{
    public  class GetTechnicianPricingForProblemTypeQuery  : IRequest<IReadOnlyList<TechnicianPricingDto>>
    {
        public int  TechnicianId { get; set; }
    }
}
