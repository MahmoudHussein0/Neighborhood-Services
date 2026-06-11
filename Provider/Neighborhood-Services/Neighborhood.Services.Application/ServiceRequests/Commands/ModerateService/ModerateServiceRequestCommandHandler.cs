using MediatR;
using Microsoft.Extensions.Logging;
using Neighborhood.Services.Application.AI.DTOs;
using Neighborhood.Services.Application.AI.Interfaces;
using Neighborhood.Services.Application.Customers.Interfaces;
using Neighborhood.Services.Application.Notifications.Services;
using Neighborhood.Services.Application.ServiceRequests.Interfaces;
using Neighborhood.Services.Application.Shared;
using Neighborhood.Services.Domain.AgentLogs;
using Neighborhood.Services.Domain.ServiceRequests;
using System.Text.Json;

namespace Neighborhood.Services.Application.ServiceRequests.Commands.ModerateService
{
    public class ModerateServiceRequestCommandHandler : IRequestHandler<ModerateServiceRequestCommand, bool>
    {
        private readonly IServiceRequestRepository _serviceRequestRepository;
        private readonly IUnitOfWork _unitOfWork;
        private readonly IAiClient _aiClient;
        private readonly ICustomerRepository _customerRepository;
        private readonly INotificationService _notificationService;
        private readonly ILogger<ModerateServiceRequestCommandHandler> _logger;

        public ModerateServiceRequestCommandHandler(
            IServiceRequestRepository serviceRequestRepository,
            IUnitOfWork unitOfWork,
            IAiClient aiClient,
            ICustomerRepository customerRepository,
            INotificationService notificationService,
            ILogger<ModerateServiceRequestCommandHandler> logger)
        {
            _serviceRequestRepository = serviceRequestRepository;
            _unitOfWork = unitOfWork;
            _aiClient = aiClient;
            _customerRepository = customerRepository;
            _notificationService = notificationService;
            _logger = logger;
        }

        public async Task<bool> Handle(ModerateServiceRequestCommand request, CancellationToken cancellationToken)
        {
            // 1. Load the request. If it's gone or already decided, do nothing (idempotent —
            //    Hangfire may run a job more than once).
            var serviceRequest = await _serviceRequestRepository.GetByIdAsync(request.ServiceRequestId);
            if (serviceRequest is null || serviceRequest.Status != ServiceRequestStatus.PendingReview)
                return serviceRequest?.Status == ServiceRequestStatus.Open;

            // 2. Build the moderation prompt. Works for Arabic or English descriptions.
            var systemPrompt = """
                You are a content-moderation agent for a home-services marketplace where customers
                post repair/maintenance requests (text description + optional photo of the problem).

                Decide whether the content is APPROPRIATE for a public, family-friendly marketplace.
                Mark it INAPPROPRIATE only if it clearly contains any of:
                - sexual or pornographic content
                - graphic violence, gore, or threats
                - hate speech, harassment, or slurs
                - illegal activity (drugs, weapons sales, etc.)
                - obvious scams or spam unrelated to a home service

                A normal home-repair description or a photo of a broken/dirty/damaged household item
                (leaks, cracks, rust, mold, broken appliances, pests, etc.) is APPROPRIATE — do NOT
                flag it just because it looks messy, dirty, or shows minor blood/rust from an injury
                to a pipe or wall. When unsure, treat it as appropriate.

                Respond in the same language as the description for the "reason" field.
                Return ONLY a valid JSON object, no extra text, in this exact format:
                {
                    "isAppropriate": true,
                    "reason": "short explanation"
                }
                """;

            var userPrompt = $"""
                Description: {serviceRequest.Description}
                """;

            // 3. Call the agent. The image (if any) is moderated by the same vision call.
            //    On ANY failure (OpenAI down, bad key, unparsable response) we FAIL OPEN:
            //    the post goes live. Moderation is best-effort; availability wins.
            bool isAppropriate;
            try
            {
                var rawResponse = await _aiClient.CompleteAsync(
                    systemPrompt,
                    userPrompt,
                    serviceRequest.Image,
                    new AiCallContext
                    {
                        AgentType = AgentType.Moderation,
                        Action = "ModerateServiceRequest",
                        ReferenceType = AgentLogReferenceType.ServiceRequest,
                        ReferenceId = serviceRequest.Id
                    });

                var json = rawResponse
                    .Replace("```json", "")
                    .Replace("```", "")
                    .Trim();

                var verdict = JsonSerializer.Deserialize<ModerationVerdict>(json, new JsonSerializerOptions
                {
                    PropertyNameCaseInsensitive = true
                });

                // No parseable verdict -> treat as appropriate (fail open).
                isAppropriate = verdict?.IsAppropriate ?? true;
            }
            catch
            {
                // LLM unreachable / errored -> fail open so the marketplace keeps working.
                isAppropriate = true;
            }

            serviceRequest.Status = isAppropriate
                ? ServiceRequestStatus.Open
                : ServiceRequestStatus.Flagged;

            await _unitOfWork.SaveChangesAsync(cancellationToken);

            // Notify — best effort. A notification failure must never undo moderation.
            try
            {
                if (isAppropriate)
                {
                    var customer = await _customerRepository.GetByIdAsync(serviceRequest.CustomerId);
                    if (!string.IsNullOrEmpty(customer?.ApplicationUserId))
                        await _notificationService.SendNotificationToUser(
                            customer.ApplicationUserId,
                            $"Your service request #{serviceRequest.Id} is now live and visible to technicians.");
                }
                else
                {
                    // Flagged — alert staff to review it in the moderation queue.
                    await _notificationService.SendNotificationToAdmin(
                        $"Service request #{serviceRequest.Id} was flagged by moderation and needs review.");
                }
            }
            catch (Exception ex)
            {
                _logger.LogWarning(ex, "Moderation notification failed for request {Id}.", serviceRequest.Id);
            }

            return isAppropriate;
        }

        private sealed class ModerationVerdict
        {
            public bool IsAppropriate { get; set; }
            public string? Reason { get; set; }
        }
    }
}
