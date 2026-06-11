using Neighborhood.Services.Application.Technicians.DTOs;

namespace Neighborhood.Services.Application.Matching.DTOs
{
    // A single ranked technician suggestion for a service request.
    public class TechnicianMatchDto
    {
        public int Rank { get; set; }
        public double Score { get; set; }              // 0–100 rule-based fit score (always computed)
        public string Reason { get; set; } = string.Empty; // why this tech fits (LLM prose, or templated on fallback)
        public TechnicianCardDTO Technician { get; set; } = default!;
    }

    // The whole match result for a request.
    public class TechnicianMatchResultDto
    {
        // True when the LLM ranked/explained the matches; false when we fell back to pure rules.
        public bool RankedByAi { get; set; }
        public List<TechnicianMatchDto> Matches { get; set; } = new();
    }
}
