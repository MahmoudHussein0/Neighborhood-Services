using MediatR;
using Neighborhood.Services.Application.ProblemTypes.DTOs;
using System;
using System.Collections.Generic;
using System.Text;

namespace Neighborhood.Services.Application.ProblemTypes.Queries
{
    public  class GetProblemTypeByIdQuery : IRequest<ProblemTypeDetailsDto>
    {
        public int  Id { get; set; }
        public GetProblemTypeByIdQuery(int id)
        {
            Id = id;
        }

    }
}
