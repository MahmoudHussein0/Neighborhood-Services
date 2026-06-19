using MediatR;
using System;
using System.Collections.Generic;
using System.Text;

namespace Neighborhood.Services.Application.TechnitianCategory.Commands
{
    public class DeleteCategortFromTechnicianCommand : IRequest<bool>
    {


        public int Id { get; set; }

        public DeleteCategortFromTechnicianCommand(int id)
        {
           Id = id;
        }
    }

}
