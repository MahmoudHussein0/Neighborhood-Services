using Neighborhood.Services.Application.Shared;
using Neighborhood.Services.Domain.AgentLogs;
using System;
using System.Collections.Generic;
using System.Text;

namespace Neighborhood.Services.Application.AgentLogs.Interfaces
{
    public interface IAgentLogRepository : IGenericRepository<AgentLog, int>
    {
        Task<IEnumerable<AgentLog>> GetByAgentTypeAsync(AgentType agentType);
        Task<IEnumerable<AgentLog>> GetByReferenceAsync(int referenceId, AgentLogReferenceType referenceType);
    }
}
