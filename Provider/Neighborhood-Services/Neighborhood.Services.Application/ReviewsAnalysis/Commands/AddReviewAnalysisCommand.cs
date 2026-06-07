using MediatR;
using System;
using System.Collections.Generic;
using System.Text;

namespace Neighborhood.Services.Application.ReviewsAnalysis.Commands
{
    public class AddReviewAnalysisCommand : IRequest<int>
    {
        public int ReviewId { get; set; }
        public  string ReviewText { get; set; }
    }
}
