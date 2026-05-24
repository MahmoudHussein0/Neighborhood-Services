namespace Neighborhood.Services.Domain.Staffs;

public class Staff
{
    public int Id { get; private set; }
    public int UserId { get; private set; }
    public StaffRole Role { get; private set; }
    public bool IsActive { get; private set; }
    public int? CreatedByStaffId { get; private set; }
    public DateTime CreatedAt { get; private set; }

    // Navigation Property
    public ICollection<StaffPermission> Permissions { get; private set; }

    // Empty Constructor For EF Core
    private Staff() { }

    // Main Constructor
    public Staff(
        int userId,
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