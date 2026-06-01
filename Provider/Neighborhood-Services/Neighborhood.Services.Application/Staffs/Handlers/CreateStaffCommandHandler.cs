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

        public CreateStaffCommandHandler(IStaffRepository repository, IUnitOfWork unitOfWork, ICurrentUserService currentUser)
        {
            _repository = repository;
            _unitOfWork = unitOfWork;
            _currentUser = currentUser;
        }

        public async Task<StaffDto> Handle(CreateStaffCommand request, CancellationToken cancellationToken)
        {
            var staff = new Staff
            {
                UserId = _currentUser.UserId,
                Role = request.Role,
                IsActive = true,
                CreatedByStaffId = request.CreatedByStaffId,
                CreatedAt = DateTime.UtcNow,
                Permissions = request.Permissions.Select(p => new StaffPermission
                {
                    Permission = p
                }).ToList()
            };

            await _repository.AddAsync(staff);
            await _unitOfWork.SaveChangesAsync();

            return StaffMapper.MapToDto(staff);
        }
    }
}
