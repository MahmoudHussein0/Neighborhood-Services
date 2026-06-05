using MediatR;
using System;
using System.Collections.Generic;
using System.Text;

namespace Neighborhood.Services.Application.AvilabilitiesException.Commands
{
    public class DeleteAvailabilityExceptionCommand : IRequest<bool>
    {
        

        public int  Id { get; set; }

        public DeleteAvailabilityExceptionCommand(int id)
        {
            Id = id;
        }
    }
}
