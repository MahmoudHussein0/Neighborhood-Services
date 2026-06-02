using MediatR;
using System;
using System.Collections.Generic;
using System.Text;

namespace Neighborhood.Services.Application.TechnitianAvailability.Commands
{
    public class DeleteTechnicianAvailabilityCommand : IRequest<bool>
    {
        public int Id { get; set; }
    }
}
