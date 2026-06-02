using Neighborhood.Services.Domain.Shared;

namespace Neighborhood.Services.Domain.Staffs;

public class Staff:BaseEntity<int>
{
   
    public string UserId { get; set; }
    public StaffRole Role { get; set; }
    public bool IsActive { get;  set; }
    public int? CreatedByStaffId { get;  set; }
    public DateTime CreatedAt { get;  set; }

    // Navigation Property
    public ICollection<StaffPermission> Permissions { get;  set; }

    // Empty Constructor For EF Core
    public Staff() { }

    // Main Constructor
    public Staff(
        string userId,
        StaffRole role,
        bool isActive,
        int? createdByStaffId,
        DateTime createdAt)
    {
        UserId = userId;
        Role = role;
        IsActive = isActive;
        CreatedByStaffId = createdByStaffId;
        CreatedAt = createdAt;
    }
}