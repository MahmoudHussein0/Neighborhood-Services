using Neighborhood.Services.Domain.Shared;

namespace Neighborhood.Services.Domain.Staffs
{
    public class StaffPermission: BaseEntity<int>
    {
      

        public int StaffId { get; set; }

        public PermissionType Permission { get;  set; }


        // Navigation Property
        public Staff Staff { get;  set; }
    }
}
