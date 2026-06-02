using Neighborhood.Services.Application.Staffs.DTOs;
using Neighborhood.Services.Domain.Staffs;

namespace Neighborhood.Services.Application.Shared.Mappers
{
    public static class StaffMapper
    {
        public static StaffDto MapToDto(Staff staff) => new()
        {
            Id = staff.Id,
            UserId = staff.UserId,
            Role = staff.Role.ToString(),
            IsActive = staff.IsActive,
            CreatedByStaffId = staff.CreatedByStaffId,
            CreatedAt = staff.CreatedAt,
            Permissions = staff.Permissions?.Select(p => new StaffPermissionDto
            {
                Id = p.Id,
                StaffId = p.StaffId,
                Permission = p.Permission.ToString()
            }).ToList() ?? new()
        };
    }
}
