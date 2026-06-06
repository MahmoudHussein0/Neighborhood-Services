using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Migrations;
using Neighborhood.Services.Application.HistoricalPrices.Interfaces;
using Neighborhood.Services.Domain.HistoricalPrices;
using Neighborhood.Services.Infrastructure.Persistence.Context;
using Neighborhood.Services.Infrastructure.Shared;
using System.Linq.Expressions;


namespace Neighborhood.Services.Infrastructure.Persistence.HistoricalPrices
{
    public class HistoricalPriceRepository : GenericRepository<HistoricalPrice, int>, IHistoricalPriceRepository
    {
        public HistoricalPriceRepository(ApplicationDbContext context) : base(context)
        { }

        public async Task<IReadOnlyList<HistoricalPrice>> GetByProblemType(int problemId)
        => (await GetByConditionAsync(Hp => Hp.ProblemTypeId == problemId)).ToList();


        public async Task<IReadOnlyList<HistoricalPrice>> GetByProblemTypeAndRegion(int problemId, string region)
          =>  (await GetByConditionAsync(Hp => Hp.ProblemTypeId == problemId && Hp.Region.Trim().ToLower() == region.Trim().ToLower())).ToList();
    }
}
