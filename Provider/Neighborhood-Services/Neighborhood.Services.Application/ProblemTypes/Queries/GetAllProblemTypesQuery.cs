using MediatR;
using Neighborhood.Services.Application.ProblemTypes.DTOs;
using System;
using System.Collections.Generic;
using System.Text;

namespace Neighborhood.Services.Application.ProblemTypes.Queries
{
    public class GetAllProblemTypesQuery  : IRequest< IReadOnlyList<ProblemTypeDto>>
    {
     
        public string? SearchTerm  { get; set; }

        public decimal?  MinPrice { get; set; }
        public decimal?  MaxPrice { get; set; }

        public GetAllProblemTypesQuery(string? searchTerm = null , decimal? minPrice = null , decimal? maxPrice = null )
        {
            SearchTerm = searchTerm;
            MinPrice = minPrice;
            MaxPrice = maxPrice;
        }


    }
}
