using MediatR;
using Neighborhood.Services.Domain.AgentLogs;

namespace Neighborhood.Services.Application.AgentLogs.Commands
{
    // Internal command — called by AI agent handlers after completing their work.
    // Not exposed as a public API endpoint.
    public class CreateAgentLogCommand : IRequest
    {
        public AgentType AgentType { get; set; }
        public string Action { get; set; } = string.Empty;
        public string Input { get; set; } = string.Empty;
        public string Output { get; set; } = string.Empty;
        public AgentLogReferenceType ReferenceType { get; set; }
        public int? ReferenceId { get; set; }
        public int? TokensUsed { get; set; }
    }
}
