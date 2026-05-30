using Neighborhood.Services.Application.AgentLogs.Interfaces;
using Neighborhood.Services.Domain.AgentLogs;
using Neighborhood.Services.Infrastructure.Persistence.Context;
using Neighborhood.Services.Infrastructure.Shared;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Text;

namespace Neighborhood.Services.Infrastructure.Persistence.AgentLogs
{
    public class AgentLogRepository :GenericRepository<AgentLog,int>,IAgentLogRepository
    {
        public AgentLogRepository(ApplicationDbContext context) : base(context)
        {
        }

        public async Task<IEnumerable<AgentLog>> GetByAgentTypeAsync(AgentType agentType)
        {
            return await _context.AgentLogs
                .Where(a => a.AgentType == agentType)
                .OrderByDescending(a => a.CreatedAt)
                .ToListAsync();
        }

        public async Task<IEnumerable<AgentLog>> GetByReferenceAsync(int referenceId, AgentLogReferenceType referenceType)
        {
            return await _context.AgentLogs
                .Where(a => a.ReferenceId == referenceId
                    && a.ReferenceType == referenceType)
                .OrderByDescending(a => a.CreatedAt)
                .ToListAsync();
        }

    }
}
