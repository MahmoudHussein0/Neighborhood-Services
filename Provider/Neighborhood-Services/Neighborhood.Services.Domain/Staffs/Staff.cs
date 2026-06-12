using Neighborhood.Services.Domain.ApplicationUsers;
using Neighborhood.Services.Domain.Disputes;
using Neighborhood.Services.Domain.Shared;

namespace Neighborhood.Services.Domain.Staffs;

public class Staff:BaseEntity<int>
{
   
    public string UserId { get; set; }
    public StaffRole Role { get; set; }
    public bool IsActive { get;  set; }
    public int? CreatedByStaffId { get;  set; }
    public DateTime CreatedAt { get; set; }

    public DateTime UpdatedAt { get; set; }

    // Navigation Property
    public ICollection<StaffPermission> Permissions { get; set; }
    = new List<StaffPermission>();


    public ICollection<Dispute> ResolvedDisputes { get; set; }
    = new List<Dispute>();
    public Staff? CreatedByStaff { get; set; }

    public ICollection<Staff> CreatedStaffs { get; set; }
        = new List<Staff>();

    public ApplicationUser User { get; set; } = null!;

    // Empty Constructor For EF Core
    public Staff() { }

  
   
}