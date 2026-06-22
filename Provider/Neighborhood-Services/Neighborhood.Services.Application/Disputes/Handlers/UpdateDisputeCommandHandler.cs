using MediatR;
using Neighborhood.Services.Application.Disputes.Commands;
using Neighborhood.Services.Application.Disputes.DTOs;
using Neighborhood.Services.Application.Disputes.Interfaces;
using Neighborhood.Services.Application.Shared;
using Neighborhood.Services.Application.Staffs.Interfaces;
using Neighborhood.Services.Domain.Disputes;
using Neighborhood.Services.Application.Shared.Mappers;

namespace Neighborhood.Services.Application.Disputes.Handlers
{
    public class UpdateDisputeCommandHandler
     : IRequestHandler<UpdateDisputeCommand, DisputeDto>
    {
        private readonly IDisputeRepository _repository;
        private readonly IUnitOfWork _unitOfWork;
        private readonly ICurrentUserService _currentUserService;
        private readonly IStaffRepository _staffRepository;

        public UpdateDisputeCommandHandler(
            IDisputeRepository repository,
            IUnitOfWork unitOfWork,
            ICurrentUserService currentUserService,
            IStaffRepository staffRepository)
        {
            _repository = repository;
            _unitOfWork = unitOfWork;
            _currentUserService = currentUserService;
            _staffRepository = staffRepository;
        }

        public async Task<DisputeDto> Handle(
            UpdateDisputeCommand request,
            CancellationToken cancellationToken)
        {
            var dispute = await _repository.GetByIdAsync(request.Id);

            if (dispute is null)
                throw new Exception($"Dispute with id {request.Id} not found.");

            // 1. Validate status transition
            if (!IsValidTransition(dispute.Status, request.Status))
            {
                throw new Exception(
                    $"Invalid dispute status transition from {dispute.Status} to {request.Status}");
            }

            // 2. Apply status
            dispute.Status = request.Status;

            // 3. Optional resolution
            if (!string.IsNullOrWhiteSpace(request.Resolution))
            {
                dispute.Resolution = request.Resolution;
            }

            // 4. When resolving → enforce rules
            if (request.Status == DisputeStatus.Resolved)
            {
                if (string.IsNullOrWhiteSpace(request.Resolution))
                    throw new Exception("Resolution is required when resolving a dispute.");

                // Always derive the resolving staff from the authenticated user rather
                // than trusting a client-supplied id (the frontend has no reliable Staff
                // id, and the request value was being hardcoded to 1).
                var userId = _currentUserService.UserId;
                var staff = string.IsNullOrWhiteSpace(userId)
                    ? null
                    : await _staffRepository.GetByUserIdAsync(userId);

                if (staff is null)
                    throw new Exception("Could not determine the resolving staff member.");

                dispute.ResolvedByStaffId = staff.Id;
                dispute.ResolvedAt = DateTime.UtcNow;
            }

            // 5. Save
            await _repository.UpdateAsync(dispute);
            await _unitOfWork.SaveChangesAsync();

            return DisputeMapper.MapToDto(dispute);
        }

        // 🔥 Business Rule: valid status transitions only
        private static bool IsValidTransition(
            DisputeStatus currentStatus,
            DisputeStatus newStatus)
        {
            if (currentStatus == newStatus)
                return true;

            return currentStatus switch
            {
                DisputeStatus.Open =>
                    newStatus == DisputeStatus.UnderReview
                    || newStatus == DisputeStatus.Resolved,

                DisputeStatus.UnderReview =>
                    newStatus == DisputeStatus.Resolved,

                DisputeStatus.Resolved =>
                    false,

                _ => false
            };
        }
    }
}