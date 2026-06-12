using Neighborhood.Services.Application.Staffs.DTOs;
using Neighborhood.Services.Domain.Staffs;

namespace Neighborhood.Services.Application.Shared.Mappers
{
    public static class StaffMapper
    {
        public static StaffDto MapToDto(Staff staff) => new()
        {
            Id = staff.Id,
            ApplicationUserId = staff.UserId,
            StaffRole = staff.Role.ToString(),
            IsActive = staff.IsActive,
            CreatedAt = staff.CreatedAt,
            Permissions = staff.Permissions?.Select(p => new StaffPermissionDto
            {
                Id = p.Id,
                StaffId = p.StaffId,
                Permission = p.Permission.ToString()
            }).ToList() ?? new(),

            FullName = staff.User?.FullName ?? string.Empty,
            Email = staff.User?.Email ?? string.Empty
        };

    }
}
