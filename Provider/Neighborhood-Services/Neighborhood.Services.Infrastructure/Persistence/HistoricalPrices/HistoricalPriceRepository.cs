using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Migrations;
using Neighborhood.Services.Application.HistoricalPrices;
using Neighborhood.Services.Domain.HistoricalPrices;
using Neighborhood.Services.Infrastructure.Persistence.Context;


namespace Neighborhood.Services.Infrastructure.Persistence.HistoricalPrices
{
    public class HistoricalPriceRepository : IHistoricalPriceRepository
    {
        private readonly ApplicationDbContext _context;

        public HistoricalPriceRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<IReadOnlyList<HistoricalPrice>> GetAllAsync()
        =>  await _context.Set<HistoricalPrice>().ToListAsync();

        public async  Task<HistoricalPrice> GetByIdAsync(int id)
         => await _context.Set<HistoricalPrice>().FindAsync(id);
    }
}
