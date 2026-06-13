using System;
using System.Collections.Generic;
using System.Text;

namespace Neighborhood.Services.Application.QA.DTOs
{
    public class DisputeAnalysisDto
    {
        public string Severity { get; set; } = string.Empty;
        public bool RequiresHumanReview { get; set; }
    }
}
