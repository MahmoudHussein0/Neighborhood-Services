using MediatR;
using Neighborhood.Services.Application.TechnitianAvailability.DTOs;
using System;
using System.Collections.Generic;
using System.Text;

namespace Neighborhood.Services.Application.TechnitianAvailability.Queries
{
    public class GetTechAvailabilityForTechnicianQuery : IRequest< IReadOnlyList<TechnicianAvailabilityDetailsDTO>>
    {
       

    }
}
