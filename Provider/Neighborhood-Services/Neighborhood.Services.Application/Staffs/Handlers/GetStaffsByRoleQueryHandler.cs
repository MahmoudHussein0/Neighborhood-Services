using MediatR;
using Neighborhood.Services.Application.Shared.Mappers;
using Neighborhood.Services.Application.Staffs.DTOs;
using Neighborhood.Services.Application.Staffs.Interfaces;
using Neighborhood.Services.Application.Staffs.Queries;

namespace Neighborhood.Services.Application.Staffs.Handlers
{
    /// <summary>
    /// Query handler for retrieving staff members by role with their ApplicationUser details
    /// Returns StaffDto with FullName and Email populated from ApplicationUser
    /// </summary>
    public class GetStaffsByRoleQueryHandler : IRequestHandler<GetStaffsByRoleQuery, IEnumerable<StaffDto>>
    {
        private readonly IStaffRepository _staffRepository;

        public GetStaffsByRoleQueryHandler(IStaffRepository staffRepository)
        {
            _staffRepository = staffRepository;
        }

        public async Task<IEnumerable<StaffDto>> Handle(GetStaffsByRoleQuery request, CancellationToken cancellationToken)
        {
            var staffs = await _staffRepository.GetByRoleAsync(request.Role);

            return staffs.Select(StaffMapper.MapToDto).ToList();
        }
    }
}
