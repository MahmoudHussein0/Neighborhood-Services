using MediatR;
using Neighborhood.Services.Application.Staffs.DTOs;
using Neighborhood.Services.Application.Staffs.Interfaces;
using Neighborhood.Services.Application.Staffs.Queries;
using Neighborhood.Services.Application.Shared.Mappers;
namespace Neighborhood.Services.Application.Staffs.Handlers
{
    public class GetStaffByIdQueryHandler : IRequestHandler<GetStaffByIdQuery, StaffDto>
    {
        private readonly IStaffRepository _repository;

        public GetStaffByIdQueryHandler(IStaffRepository repository)
        {
            _repository = repository;
        }

        public async Task<StaffDto> Handle(GetStaffByIdQuery request, CancellationToken cancellationToken)
        {
            var staff = await _repository.GetByIdAsync(request.Id, cancellationToken);
            if (staff is null)
                throw new Exception($"Staff with id {request.Id} not found.");

            return StaffMapper.MapToDto(staff);
        }
    }
}
