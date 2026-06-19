using MediatR;
using Neighborhood.Services.Application.Shared;
using Neighborhood.Services.Application.Shared.Mappers;
using Neighborhood.Services.Application.Staffs.Commands;
using Neighborhood.Services.Application.Staffs.DTOs;
using Neighborhood.Services.Application.Staffs.Interfaces;
using Neighborhood.Services.Domain.Staffs;

namespace Neighborhood.Services.Application.Staffs.Handlers
{
    public class UpdateStaffCommandHandler : IRequestHandler<UpdateStaffCommand, StaffDto>
    {
        private readonly IStaffRepository _repository;
        private readonly IUnitOfWork _unitOfWork;
        private readonly ICurrentUserService _currentUser;

        public UpdateStaffCommandHandler(
            IStaffRepository repository,
            IUnitOfWork unitOfWork,
            ICurrentUserService currentUser)
        {
            _repository = repository;
            _unitOfWork = unitOfWork;
            _currentUser = currentUser;
        }

        public async Task<StaffDto> Handle(
            UpdateStaffCommand request,
            CancellationToken cancellationToken)
        {
            var currentStaff = await _repository.GetByUserIdAsync(
                _currentUser.UserId);

            if (currentStaff is null)
                throw new UnauthorizedAccessException("Current staff not found.");

            var staff = await _repository.GetByIdAsync(request.Id);

            if (staff is null)
                throw new Exception($"Staff with id {request.Id} not found.");

            staff.Role = request.Role;
            staff.IsActive = request.IsActive;

            // ✅ نفس المنطق للـ Admin والـ TechnicalSupport
            var permissions = request.Permissions.Distinct();

            if (!permissions.Any())
                throw new Exception("Staff must have at least one permission.");

            // ✅ امسح القديم واحفظ الأول
            await _repository.ReplacePermissionsAsync(
                staff.Id,
                permissions,
                cancellationToken);

            await _unitOfWork.SaveChangesAsync();

            // ✅ بعدين عدّل الـ staff واحفظ تاني
            await _repository.UpdateAsync(staff);
            await _unitOfWork.SaveChangesAsync();

            // ✅ جيب fresh copy من الداتابيز عشان الـ DTO يكون محدث
            var updatedStaff = await _repository.GetByIdAsync(request.Id);
            return StaffMapper.MapToDto(updatedStaff!);
        }
    }
}