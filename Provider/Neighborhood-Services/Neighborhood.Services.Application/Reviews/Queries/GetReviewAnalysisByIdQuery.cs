using MediatR;
using Neighborhood.Services.Application.Reviews.DTOs;
using System;
using System.Collections.Generic;
using System.Text;

namespace Neighborhood.Services.Application.Reviews.Queries
{
    public class GetReviewAnalysisByIdQuery : IRequest<ReviewAnalysisDto>
    {
        public int Id { get; set; }
    }
}
