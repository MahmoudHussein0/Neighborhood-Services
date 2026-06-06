using Neighborhood.Services.Domain.Bookings;
using Neighborhood.Services.Domain.Shared;
using System;
using System.Collections.Generic;
using System.Text;

namespace Neighborhood.Services.Domain.AiAnalyses
{
    public class AiAnalysis :BaseEntity<int>
    {
        //---- Self prop
        public string DetectedProblem { get; set; } = string.Empty;
        public decimal ConfidenceScore { get; set; }
        public decimal EstimatedMinPrice { get; set; }
        public decimal EstimatedMaxPrice { get; set; }
        public SeverityLevel SeverityLevel { get; set; }
        public DateTime GeneratedAt { get; set; }

        //---- Foreign Keys
        public int? BookingId { get; set; }

        // Nav
        public Booking? Booking { get; set; }

    }
}
