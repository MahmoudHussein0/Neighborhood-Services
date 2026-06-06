using MediatR;
using Neighborhood.Services.Application.Staffs.DTOs;
using Neighborhood.Services.Domain.Staffs;

namespace Neighborhood.Services.Application.Staffs.Commands
{
    public class CreateStaffCommand : IRequest<StaffDto>
    {
        public string UserId { get; set; }
        public StaffRole Role { get; set; }
        public int? CreatedByStaffId { get; set; }
        public List<PermissionType> Permissions { get; set; } = new();
    }
}
