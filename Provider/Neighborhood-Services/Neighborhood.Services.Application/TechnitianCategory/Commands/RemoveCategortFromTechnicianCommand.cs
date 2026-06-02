using MediatR;
using System;
using System.Collections.Generic;
using System.Text;

namespace Neighborhood.Services.Application.TechnitianCategory.Commands
{
    public class RemoveCategortFromTechnicianCommand : IRequest<bool>
    {
        public int TechnicianId { get; set; }
        public int CategoryId { get; set; }
    }
}
