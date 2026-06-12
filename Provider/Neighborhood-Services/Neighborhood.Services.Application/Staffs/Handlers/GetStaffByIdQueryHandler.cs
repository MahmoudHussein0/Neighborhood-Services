using MediatR;
using Neighborhood.Services.Application.Shared.Mappers;
using Neighborhood.Services.Application.Staffs.DTOs;
using Neighborhood.Services.Application.Staffs.Interfaces;
using Neighborhood.Services.Application.Staffs.Queries;
using Neighborhood.Services.Domain.Staffs;

namespace Neighborhood.Services.Application.Staffs.Handlers
{
    /// <summary>
    /// Query handler for retrieving a single staff member by ID with their ApplicationUser details
    /// Returns StaffDto with FullName and Email populated from ApplicationUser
    /// </summary>
    public class GetStaffByIdQueryHandler : IRequestHandler<GetStaffByIdQuery, StaffDto>
    {
        private readonly IStaffRepository _staffRepository;

        public GetStaffByIdQueryHandler(IStaffRepository staffRepository)
        {
            _staffRepository = staffRepository;
        }

        public async Task<StaffDto> Handle(GetStaffByIdQuery request, CancellationToken cancellationToken)
        {
            var staff = await _staffRepository.GetByIdAsync(request.Id);
            if (staff is null)
                throw new Exception($"Staff with id {request.Id} not found.");

            return StaffMapper.MapToDto(staff);
        }
    }
}
