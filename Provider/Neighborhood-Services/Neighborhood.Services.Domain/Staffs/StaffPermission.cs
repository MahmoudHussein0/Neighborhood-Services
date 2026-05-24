namespace Neighborhood.Services.Domain.Staffs
{
    public class StaffPermission
    {
        public int Id { get; private set; }

        public int StaffId { get; private set; }

        public PermissionType Permission { get; private set; }


        // Navigation Property
        public Staff Staff { get; private set; }
    }
}
