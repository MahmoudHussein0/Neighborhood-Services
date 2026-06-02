using MediatR;
using Neighborhood.Services.Application.Staffs.DTOs;

namespace Neighborhood.Services.Application.Staffs.Queries
{
    public class GetAllStaffsQuery : IRequest<IReadOnlyList<StaffDto>>
    {
    }

}
