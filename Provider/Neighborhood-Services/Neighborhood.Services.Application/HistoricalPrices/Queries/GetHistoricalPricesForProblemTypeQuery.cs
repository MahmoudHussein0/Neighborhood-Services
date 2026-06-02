using MediatR;
using Neighborhood.Services.Application.HistoricalPrices.DTOs;
using System;
using System.Collections.Generic;
using System.Text;

namespace Neighborhood.Services.Application.HistoricalPrices.Queries
{
    public  class GetHistoricalPricesForProblemTypeQuery : IRequest<IReadOnlyList<HistoricalPricingDto>>
    {
        public int ProblemTypeId { get; set; }
    }
}
