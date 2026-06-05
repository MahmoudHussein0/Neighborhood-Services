using MediatR;
using Neighborhood.Services.Application.ProblemTypes.DTOs;
using System;
using System.Collections.Generic;
using System.Text;

namespace Neighborhood.Services.Application.ProblemTypes.Queries
{
    public  class GetProblemTypeByIdQuery : IRequest<ProblemTypeDetailsDto>
    {

        public string Lang  { get; set; }
        public int  Id { get; set; }

        public GetProblemTypeByIdQuery(string lang, int id)
        {
            Lang = lang;
            Id = id;
        }


    }
}
