using MediatR;
using Neighborhood.Services.Application.Staffs.DTOs;
using Neighborhood.Services.Application.Staffs.Interfaces;
using Neighborhood.Services.Application.Staffs.Queries;
using Neighborhood.Services.Application.Shared.Mappers;

namespace Neighborhood.Services.Application.Staffs.Handlers
{
    public class GetAllStaffsQueryHandler : IRequestHandler<GetAllStaffsQuery, IReadOnlyList<StaffDto>>
    {
        private readonly IStaffRepository _repository;

        public GetAllStaffsQueryHandler(IStaffRepository repository)
        {
            _repository = repository;
        }

        public async Task<IReadOnlyList<StaffDto>> Handle(GetAllStaffsQuery request, CancellationToken cancellationToken)
        {
            var staffs = await _repository.GetAllAsync(cancellationToken);
            return staffs.Select(StaffMapper.MapToDto).ToList();
        }
    }

}
