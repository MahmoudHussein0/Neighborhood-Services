using MediatR;
using Neighborhood.Services.Application.ProblemTypes.DTOs;
using System;
using System.Collections.Generic;
using System.Text;

namespace Neighborhood.Services.Application.ProblemTypes.Commands
{
    public class UpdateProblemTypeCommand : IRequest<UpdateProblemTypeDto>
    {
        public int  Id { get; set; }
        public string Description { get; set; }
        public decimal MinPrice { get; set; }
        public decimal MaxPrice { get; set; }
    }
}
