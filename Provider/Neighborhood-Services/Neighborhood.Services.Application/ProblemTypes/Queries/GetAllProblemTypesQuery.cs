using MediatR;
using Neighborhood.Services.Application.ProblemTypes.DTOs;
using System;
using System.Collections.Generic;
using System.Text;

namespace Neighborhood.Services.Application.ProblemTypes.Queries
{
    public class GetAllProblemTypesQuery  : IRequest< IReadOnlyList<ProblemTypeDto>>
    {

        public string Lang  { get; set; }
        public string? SearchTerm  { get; set; }
        public decimal?  MinPrice { get; set; }
        public decimal?  MaxPrice { get; set; }

        public GetAllProblemTypesQuery(string lang, string? searchTerm, decimal? minPrice, decimal? maxPrice)
        {
            Lang = lang;
            SearchTerm = searchTerm;
            MinPrice = minPrice;
            MaxPrice = maxPrice;
        }

    }
}
