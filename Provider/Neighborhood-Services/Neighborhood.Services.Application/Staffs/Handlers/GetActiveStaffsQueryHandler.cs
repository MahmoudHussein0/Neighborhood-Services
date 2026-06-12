using MediatR;
using Neighborhood.Services.Application.Shared.Mappers;
using Neighborhood.Services.Application.Staffs.DTOs;
using Neighborhood.Services.Application.Staffs.Interfaces;
using Neighborhood.Services.Application.Staffs.Queries;

namespace Neighborhood.Services.Application.Staffs.Handlers
{
    /// <summary>
    /// Query handler for retrieving active staff members with their ApplicationUser details
    /// Returns StaffDto with FullName and Email populated from ApplicationUser
    /// </summary>
    public class GetActiveStaffsQueryHandler : IRequestHandler<GetActiveStaffsQuery, IEnumerable<StaffDto>>
    {
        private readonly IStaffRepository _staffRepository;

        public GetActiveStaffsQueryHandler(IStaffRepository staffRepository)
        {
            _staffRepository = staffRepository;
        }

        public async Task<IEnumerable<StaffDto>> Handle(GetActiveStaffsQuery request, CancellationToken cancellationToken)
        {
            var staffs = await _staffRepository.GetActiveAsync();


            return staffs
                .Select(StaffMapper.MapToDto)
                .ToList();
        }
    }
}
