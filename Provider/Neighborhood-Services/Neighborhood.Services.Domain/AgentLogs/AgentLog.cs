using Neighborhood.Services.Domain.Shared;
using System;
using System.Collections.Generic;
using System.Text;

namespace Neighborhood.Services.Domain.AgentLogs
{
    public class AgentLog :BaseEntity<int>
    {
        public AgentType AgentType { get; set; }
        public string Action { get; set; } = string.Empty;
        public string Input { get; set; } = string.Empty;  // text on db
        public string Output { get; set; } = string.Empty; // text on db
        public AgentLogReferenceType ReferenceType { get; set; }
        public DateTime CreatedAt { get; set; }
        public int? ReferenceId { get; set; }
        public int? TokensUsed { get; set; }


    }
}
