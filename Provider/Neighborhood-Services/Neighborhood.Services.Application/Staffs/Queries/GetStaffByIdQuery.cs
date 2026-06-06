using MediatR;
using Neighborhood.Services.Application.Staffs.DTOs;

namespace Neighborhood.Services.Application.Staffs.Queries
{
    public class GetStaffByIdQuery : IRequest<StaffDto>
    {
        public int Id { get; set; }
        public GetStaffByIdQuery(int id) => Id = id;
    }
}
