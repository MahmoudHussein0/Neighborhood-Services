using MediatR;
using Neighborhood.Services.Application.TechnitianAvailability.DTOs;
using System;
using System.Collections.Generic;
using System.Text;

namespace Neighborhood.Services.Application.TechnitianAvailability.Commands
{
    public  class UpdateTechnicianAvailabilityCommand  :IRequest<TechnicianAvailabilityDTO>
    {


        public int Id { get; set; }

        public DayOfWeek DayOfWeek { get; set; }

        public TimeOnly  StartTime { get; set; }
        public TimeOnly  EndTime { get; set; }

        public UpdateTechnicianAvailabilityCommand(int id)
        {
            Id = id;
        }

    }
}
