using MediatR;
using System;
using System.Collections.Generic;
using System.Text;

namespace Neighborhood.Services.Application.TechnitianCategory.Commands
{
    public class DeleteCategortFromTechnicianCommand : IRequest<bool>
    {
       

        public int TechnicianId { get; set; }
        public int CategoryId { get; set; }

        public DeleteCategortFromTechnicianCommand(int technicianId, int categoryId)
        {
            TechnicianId = technicianId;
            CategoryId = categoryId;
        }
    }
}
