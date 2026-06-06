using MediatR;
using Neighborhood.Services.Application.Staffs.DTOs;
using Neighborhood.Services.Domain.Staffs;

namespace Neighborhood.Services.Application.Staffs.Commands
{
    public class UpdateStaffCommand : IRequest<StaffDto>
    {
        public int Id { get; set; }
        public StaffRole Role { get; set; }
        public bool IsActive { get; set; }
        public List<PermissionType> Permissions { get; set; } = new();
    }
}
