using MediatR;
using System;
using System.Collections.Generic;
using System.Text;

namespace Neighborhood.Services.Application.ReviewsAnalysis.Commands
{
    internal class AddReviewAnalysisCommandHandler : IRequestHandler<AddReviewAnalysisCommand, int>
    {







        public Task<int> Handle(AddReviewAnalysisCommand request, CancellationToken cancellationToken)
        {
            throw new NotImplementedException();
        }
    }
}
