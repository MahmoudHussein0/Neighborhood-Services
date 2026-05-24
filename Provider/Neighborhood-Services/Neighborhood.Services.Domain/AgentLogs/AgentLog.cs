using System;
using System.Collections.Generic;
using System.Text;

namespace Neighborhood.Services.Domain.AgentLogs
{
    internal class AgentLog
    {
        public int Id { get; set; }
        public AgentType AgentType { get; set; }
        public string Action { get; set; } = string.Empty;
        public string Input { get; set; } = string.Empty;
        public string Output { get; set; } = string.Empty;
        public AgentLogReferenceType ReferenceType { get; set; }
        public DateTime CreatedAt { get; set; }


    }
}
