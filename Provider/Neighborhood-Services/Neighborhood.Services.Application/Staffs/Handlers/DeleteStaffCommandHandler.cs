using MediatR;
using Neighborhood.Services.Application.Shared;
using Neighborhood.Services.Application.Staffs.Commands;
using Neighborhood.Services.Application.Staffs.Interfaces;
using Neighborhood.Services.Domain.Staffs;

namespace Neighborhood.Services.Application.Staffs.Handlers
{
    public class DeleteStaffCommandHandler : IRequestHandler<DeleteStaffCommand, bool>
    {
        private readonly IStaffRepository _repository;
        private readonly IUnitOfWork _unitOfWork;
        private readonly ICurrentUserService _currentUser;

        public DeleteStaffCommandHandler(
            IStaffRepository repository,
            IUnitOfWork unitOfWork,
            ICurrentUserService currentUser)
        {
            _repository = repository;
            _unitOfWork = unitOfWork;
            _currentUser = currentUser;
        }

        public async Task<bool> Handle(
            DeleteStaffCommand request,
            CancellationToken cancellationToken)
        {
            var currentStaff = await _repository.GetByUserIdAsync(
                _currentUser.UserId);

            if (currentStaff is null)
            {
                throw new UnauthorizedAccessException(
                    "Current staff not found.");
            }

            var hasFullAccess = await _repository.HasPermissionAsync(
                currentStaff.Id,
                PermissionType.FullAccess,
                cancellationToken);

            if (!hasFullAccess)
            {
                throw new UnauthorizedAccessException(
                    "Only Super Admin can delete staff.");
            }

            if (currentStaff.Id == request.Id)
            {
                throw new Exception(
                    "You cannot delete your own account.");
            }

            var staff = await _repository.GetByIdAsync(request.Id);

            if (staff is null)
            {
                throw new Exception(
                    $"Staff with id {request.Id} not found.");
            }

            staff.IsActive = false;
            staff.IsDeleted = true;

            await _repository.UpdateAsync(staff);
            await _unitOfWork.SaveChangesAsync();

            return true;
        }
    }
}
