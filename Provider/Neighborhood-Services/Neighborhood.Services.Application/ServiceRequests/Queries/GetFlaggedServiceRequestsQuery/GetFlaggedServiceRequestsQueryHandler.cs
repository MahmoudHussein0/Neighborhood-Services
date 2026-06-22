using MediatR;
using Neighborhood.Services.Application.AgentLogs.Interfaces;
using Neighborhood.Services.Application.ServiceRequests.DTOs;
using Neighborhood.Services.Application.ServiceRequests.Interfaces;
using Neighborhood.Services.Application.Shared;
using Neighborhood.Services.Domain.AgentLogs;
using Neighborhood.Services.Domain.ServiceRequests;
using System.Text.Json;

namespace Neighborhood.Services.Application.ServiceRequests.Queries.GetFlaggedServiceRequestsQuery
{
    public class GetFlaggedServiceRequestsQueryHandler
        : IRequestHandler<GetFlaggedServiceRequestsQuery, PagedResult<FlaggedServiceRequestDto>>
    {
        private readonly IServiceRequestRepository _serviceRequestRepository;
        private readonly IAgentLogRepository _agentLogRepository;

        public GetFlaggedServiceRequestsQueryHandler(
            IServiceRequestRepository serviceRequestRepository,
            IAgentLogRepository agentLogRepository)
        {
            _serviceRequestRepository = serviceRequestRepository;
            _agentLogRepository = agentLogRepository;
        }

        public async Task<PagedResult<FlaggedServiceRequestDto>> Handle(GetFlaggedServiceRequestsQuery request, CancellationToken cancellationToken)
        {
            var page = request.Page < 1 ? 1 : request.Page;
            var pageSize = request.PageSize is < 1 or > 100 ? 10 : request.PageSize;

            var paged = await _serviceRequestRepository.GetByStatusPagedAsync(
                ServiceRequestStatus.Flagged, page, pageSize);

            var items = new List<FlaggedServiceRequestDto>(paged.Items.Count());
            foreach (var sr in paged.Items)
            {
                items.Add(new FlaggedServiceRequestDto
                {
                    Id = sr.Id,
                    Description = sr.Description,
                    Address = sr.Address,
                    Image = sr.Image,
                    Budget = sr.Budget,
                    CustomerId = sr.CustomerId,
                    ScheduledAt = sr.ScheduledAt,
                    CreatedAt = sr.CreatedAt,
                    Reason = await GetFlagReasonAsync(sr.Id)
                });
            }

            return new PagedResult<FlaggedServiceRequestDto>(
                items, paged.TotalCount, paged.Page, paged.PageSize);
        }

        // The agent never stores the reason on the request — it lives in the moderation
        // agent log's Output (the raw verdict JSON). Pull the latest one and read "reason".
        // Best-effort: a missing or unparseable log just yields null (no reason shown).
        private async Task<string?> GetFlagReasonAsync(int serviceRequestId)
        {
            var logs = await _agentLogRepository.GetByReferenceAsync(
                serviceRequestId, AgentLogReferenceType.ServiceRequest);

            var latest = logs.FirstOrDefault(l => l.Action == "ModerateServiceRequest");
            if (latest is null || string.IsNullOrWhiteSpace(latest.Output))
                return null;

            try
            {
                var json = latest.Output.Replace("```json", "").Replace("```", "").Trim();
                using var doc = JsonDocument.Parse(json);
                if (doc.RootElement.TryGetProperty("reason", out var reason))
                    return reason.GetString();
            }
            catch
            {
                // Unparseable verdict — fall through to null.
            }

            return null;
        }
    }
}
