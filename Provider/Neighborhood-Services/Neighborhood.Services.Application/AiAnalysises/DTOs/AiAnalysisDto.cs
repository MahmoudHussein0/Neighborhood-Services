using Neighborhood.Services.Domain.AiAnalyses;
using System;
using System.Collections.Generic;
using System.Text;

namespace Neighborhood.Services.Application.AiAnalysises.DTOs
{
    public class AiAnalysisDto
    {
        public string DetectedProblem { get; set; } = string.Empty;
        public decimal ConfidenceScore { get; set; }
        public SeverityLevel SeverityLevel { get; set; }
        public decimal EstimatedMinPrice { get; set; }
        public decimal EstimatedMaxPrice { get; set; }
        public DateTime GeneratedAt { get; set; }
    }
}
