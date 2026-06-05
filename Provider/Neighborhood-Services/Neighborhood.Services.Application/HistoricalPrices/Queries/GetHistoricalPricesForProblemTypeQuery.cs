using MediatR;
using Neighborhood.Services.Application.HistoricalPrices.DTOs;
using System;
using System.Collections.Generic;
using System.Text;

namespace Neighborhood.Services.Application.HistoricalPrices.Queries
{
    public  class GetHistoricalPricesForProblemTypeQuery : IRequest<IReadOnlyList<HistoricalPricingDto>>
    {

        public string Lang { get; set; }
        public int ProblemTypeId { get; set; }

        public GetHistoricalPricesForProblemTypeQuery(int problemTypeId)
        {
            ProblemTypeId = problemTypeId;
        }
    }
}
