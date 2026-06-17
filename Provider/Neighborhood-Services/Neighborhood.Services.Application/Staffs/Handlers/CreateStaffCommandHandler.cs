using MediatR;
using Neighborhood.Services.Application.Shared;
using Neighborhood.Services.Application.Staffs.Commands;
using Neighborhood.Services.Application.Staffs.DTOs;
using Neighborhood.Services.Application.Staffs.Interfaces;
using Neighborhood.Services.Domain.Staffs;
using Neighborhood.Services.Application.Shared.Mappers;

namespace Neighborhood.Services.Application.Staffs.Handlers
{
    public class CreateStaffCommandHandler : IRequestHandler<CreateStaffCommand, StaffDto>
    {
        private readonly IStaffRepository _repository;
        private readonly IUnitOfWork _unitOfWork;
        private readonly ICurrentUserService _currentUser;

        public CreateStaffCommandHandler(
            IStaffRepository repository,
            IUnitOfWork unitOfWork,
            ICurrentUserService currentUser)
        {
            _repository = repository;
            _unitOfWork = unitOfWork;
            _currentUser = currentUser;
        }

        public async Task<StaffDto> Handle(
            CreateStaffCommand request,
            CancellationToken cancellationToken)
        {
            if (await _repository.ExistsByUserIdAsync(
                request.UserId,
                cancellationToken))
            {
                throw new Exception("User is already a staff member.");
            }

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
                    "Only Super Admin can create staff members.");
            }

            // ✅ كلا الـ Role بياخدوا الـ permissions اللي اختارها الأدمن
            var permissions = request.Permissions.Distinct().ToList();

            if (!permissions.Any())
                throw new Exception("Staff must have at least one permission.");

            if (request.Role != StaffRole.Admin &&
                request.Role != StaffRole.TechnicalSupport)
            {
                throw new Exception("Invalid staff role.");
            }

            var staff = new Staff
            {
                UserId = request.UserId,
                Role = request.Role,
                IsActive = true,
                CreatedByStaffId = currentStaff.Id,
                CreatedAt = DateTime.UtcNow,
                Permissions = permissions
                    .Select(p => new StaffPermission
                    {
                        Permission = p
                    })
                    .ToList()
            };

            await _repository.AddAsync(staff);
            await _unitOfWork.SaveChangesAsync();

            // ✅ جيب fresh copy من الداتابيز عشان الـ DTO يكون محدث
            var createdStaff = await _repository.GetByIdAsync(staff.Id);
            return StaffMapper.MapToDto(createdStaff!);
        }
    }
}