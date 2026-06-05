using MediatR;
using Neighborhood.Services.Application.AvilabilitiesException.DTOs;
using System;
using System.Collections.Generic;
using System.Text;

namespace Neighborhood.Services.Application.AvilabilitiesException.Queries
{
    public class GetAvabilityExceptionForSpecificTechQuery : IRequest<IReadOnlyList<AvailiabilityExceptionDTO>>
    {
      

        public int  TechnicianId { get; set; }

        public GetAvabilityExceptionForSpecificTechQuery(int technicianId)
        {
            TechnicianId = technicianId;
        }
    }
}
