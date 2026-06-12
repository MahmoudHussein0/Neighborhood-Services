using MediatR;
using Neighborhood.Services.Application.Shared.Mappers;
using Neighborhood.Services.Application.Staffs.DTOs;
using Neighborhood.Services.Application.Staffs.Interfaces;
using Neighborhood.Services.Application.Staffs.Queries;

namespace Neighborhood.Services.Application.Staffs.Handlers
{
    /// <summary>
    /// Query handler for retrieving a staff member by User ID with their ApplicationUser details
    /// Returns StaffDto with FullName and Email populated from ApplicationUser
    /// </summary>
    public class GetStaffByUserIdQueryHandler : IRequestHandler<GetStaffByUserIdQuery, StaffDto>
    {
        private readonly IStaffRepository _staffRepository;

        public GetStaffByUserIdQueryHandler(IStaffRepository staffRepository)
        {
            _staffRepository = staffRepository;
        }

        public async Task<StaffDto> Handle(GetStaffByUserIdQuery request, CancellationToken cancellationToken)
        {
            var staff = await _staffRepository.GetByUserIdAsync(request.UserId);

            
            if (staff is null)
                throw new Exception(
                    $"Staff with user id {request.UserId} not found.");

            return StaffMapper.MapToDto(staff);
        }
    }
}
