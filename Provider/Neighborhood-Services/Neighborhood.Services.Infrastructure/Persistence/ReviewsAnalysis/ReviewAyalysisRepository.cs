using Neighborhood.Services.Application.ReviewsAnalysis;
using Neighborhood.Services.Domain.Reviews;
using Neighborhood.Services.Infrastructure.Persistence.Context;
using Neighborhood.Services.Infrastructure.Shared;
using System;
using System.Collections.Generic;
using System.Text;

namespace Neighborhood.Services.Infrastructure.Persistence.ReviewsAnalysis
{
    public class ReviewAyalysisRepository  : GenericRepository<ReviewAnalysis , int> , IReviewAnalysisRepository
    {
        public ReviewAyalysisRepository(ApplicationDbContext context) : base(context)
        {}
    }
}
