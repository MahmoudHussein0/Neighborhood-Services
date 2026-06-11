using MediatR;
using Neighborhood.Services.Application.Matching.DTOs;

namespace Neighborhood.Services.Application.Matching.Queries
{
    // The matchmaking agent (Find Technician "Smart Match"): the customer picks a category +
    // problem type (and optionally describes the issue / shares location), and the agent returns
    // the best 1-2 technicians to Book / Set up Recurring.
    // Rules filter the candidates; the LLM ranks/explains the shortlist; on LLM failure we fall
    // back to pure-rules ranking and log the fallback to AgentLog.
    public class GetTechnicianMatchesQuery : IRequest<TechnicianMatchResultDto>
    {
        public int CategoryId { get; set; }
        public int? ProblemTypeId { get; set; }
        public double? Latitude { get; set; }
        public double? Longitude { get; set; }
        public string? Description { get; set; }   // optional free text — makes the match smarter
        public int TopN { get; set; } = 2;
    }
}
