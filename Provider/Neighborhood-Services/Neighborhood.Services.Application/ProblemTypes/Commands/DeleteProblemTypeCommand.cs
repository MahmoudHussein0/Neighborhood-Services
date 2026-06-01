using MediatR;
using System;
using System.Collections.Generic;
using System.Text;

namespace Neighborhood.Services.Application.ProblemTypes.Commands
{
    public class DeleteProblemTypeCommand : IRequest<bool>
    {
        public int  Id { get; set; }
    }
}
