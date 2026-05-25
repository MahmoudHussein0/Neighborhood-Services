using Neighborhood.Services.Domain.HistoricalPrices;
using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Text;

namespace Neighborhood.Services.Application.HistoricalPrices
{
    public interface IHistoricalPriceRepository
    {
        Task<IReadOnlyList<HistoricalPrice>> GetAllAsync();
        Task<HistoricalPrice> GetByIdAsync(int id);
    }
}
