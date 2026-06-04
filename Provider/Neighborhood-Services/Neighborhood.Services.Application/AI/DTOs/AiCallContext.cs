using Neighborhood.Services.Domain.AgentLogs;
using System;
using System.Collections.Generic;
using System.Text;

namespace Neighborhood.Services.Application.AI.DTOs
{
    public class AiCallContext
    {
        public AgentType AgentType { get; set; }
        public string Action { get; set; } = string.Empty;
        public AgentLogReferenceType ReferenceType { get; set; }
        public int? ReferenceId { get; set; }
    }
}
