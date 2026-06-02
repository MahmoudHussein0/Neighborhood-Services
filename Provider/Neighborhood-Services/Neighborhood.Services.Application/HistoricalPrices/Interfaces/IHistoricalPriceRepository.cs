using Neighborhood.Services.Application.Shared;
using Neighborhood.Services.Domain.HistoricalPrices;
using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Text;

namespace Neighborhood.Services.Application.HistoricalPrices.Interfaces
{
    public interface IHistoricalPriceRepository  : IGenericRepository<HistoricalPrice , int>
    {}
}
