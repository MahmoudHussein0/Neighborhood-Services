using System;
using System.Collections.Generic;
using System.Text;

namespace Neighborhood.Services.Domain.AiAnalyses
{
    public class AiAnalysis
    {
        //---- Self prop
        public int Id { get; set; }
        public string DetectedProblem { get; set; } = string.Empty;
        public decimal ConfidenceScore { get; set; }
        public decimal EstimatedMinPrice { get; set; }
        public decimal EstimatedMaxPrice { get; set; }
        public SeverityLevel SeverityLevel { get; set; }
        public DateTime GeneratedAt { get; set; }

        //---- Foreign Keys
        public int BookingId { get; set; }

    }
}
