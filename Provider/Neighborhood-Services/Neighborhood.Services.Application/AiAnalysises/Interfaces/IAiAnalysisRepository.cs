using Neighborhood.Services.Application.Shared;
using Neighborhood.Services.Domain.AiAnalyses;
using System;
using System.Collections.Generic;
using System.Text;

namespace Neighborhood.Services.Application.AiAnalysises.Interface
{
    public interface IAiAnalysisRepository:IGenericRepository<AiAnalysis,int>
    {
        Task<AiAnalysis?> GetByBookingIdAsync(int bookingId);
    }
}
