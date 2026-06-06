using System;
using System.Collections.Generic;
using System.Text;

namespace Neighborhood.Services.Application.AiAnalysises.DTOs
{
    public class AiAnalysisResult
    {
        public string DetectedProblem { get; set; } = string.Empty;
        public decimal ConfidenceScore { get; set; }
        public decimal EstimatedMinPrice { get; set; }
        public decimal EstimatedMaxPrice { get; set; }
        public string SeverityLevel { get; set; } = string.Empty;
    }
}
