using MediatR;
using Neighborhood.Services.Application.Shared;
using Neighborhood.Services.Application.Staffs.Commands;
using Neighborhood.Services.Application.Staffs.DTOs;
using Neighborhood.Services.Application.Staffs.Interfaces;
using Neighborhood.Services.Domain.Staffs;
using Neighborhood.Services.Application.Shared.Mappers;

namespace Neighborhood.Services.Application.Staffs.Handlers
{
    public class UpdateStaffCommandHandler : IRequestHandler<UpdateStaffCommand, StaffDto>
    {
        private readonly IStaffRepository _repository;
        private readonly IUnitOfWork _unitOfWork;

        public UpdateStaffCommandHandler(IStaffRepository repository, IUnitOfWork unitOfWork)
        {
            _repository = repository;
            _unitOfWork = unitOfWork;
        }

        public async Task<StaffDto> Handle(UpdateStaffCommand request, CancellationToken cancellationToken)
        {
            var staff = await _repository.GetByIdAsync(request.Id, cancellationToken);
            if (staff is null)
                throw new Exception($"Staff with id {request.Id} not found.");

            staff.Role = request.Role;
            staff.IsActive = request.IsActive;
            staff.Permissions = request.Permissions.Select(p => new StaffPermission
            {
                StaffId = staff.Id,
                Permission = p
            }).ToList();

            await _repository.UpdateAsync(staff);
            await _unitOfWork.SaveChangesAsync();

            return StaffMapper.MapToDto(staff);
        }
    }
}
