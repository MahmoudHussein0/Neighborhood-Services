using Neighborhood.Services.Application.AiAnalysises.Interface;
using Neighborhood.Services.Application.Shared;
using Neighborhood.Services.Domain.AiAnalyses;
using Neighborhood.Services.Infrastructure.Persistence.Context;
using Neighborhood.Services.Infrastructure.Shared;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Text;

namespace Neighborhood.Services.Infrastructure.Persistence.AiAnalysises
{
    public class AiAnalysisRepository : GenericRepository<AiAnalysis, int>, IAiAnalysisRepository
    {
        public AiAnalysisRepository(ApplicationDbContext context) : base(context)
        {
        }

        public async Task<AiAnalysis?> GetByBookingIdAsync(int bookingId)
        {
            return await _context.AiAnalyses
                .FirstOrDefaultAsync(a => a.BookingId == bookingId);
        }
    }
}
