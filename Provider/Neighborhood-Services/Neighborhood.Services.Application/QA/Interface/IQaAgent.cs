using Neighborhood.Services.Application.QA.DTOs;
using System;
using System.Collections.Generic;
using System.Text;

namespace Neighborhood.Services.Application.QA.Interface
{
    public  interface  IQaAgent
    {
        Task<ReviewAnalysisDto> AnalyzeReviewAsync(string reviewText  , int reviewId);

        Task<DisputeAnalysisDto> AnalyzeDisputeAsync(string disputeText , int disputeId);
    }
}
