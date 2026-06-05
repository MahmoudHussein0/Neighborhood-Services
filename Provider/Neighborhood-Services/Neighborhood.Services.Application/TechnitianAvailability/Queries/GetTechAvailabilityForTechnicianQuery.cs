using MediatR;
using Neighborhood.Services.Application.TechnitianAvailability.DTOs;
using System;
using System.Collections.Generic;
using System.Text;

namespace Neighborhood.Services.Application.TechnitianAvailability.Queries
{
    public class GetTechAvailabilityForTechnicianQuery : IRequest< IReadOnlyList<TechnicianAvailabilityDetailsDTO>>
    {
        public int  TechnicianId { get; set; }

        public GetTechAvailabilityForTechnicianQuery(int technicianId)
        {
            TechnicianId = technicianId;
        }


    }
}
