namespace Neighborhood.Services.Application.Staffs.DTOs
{
    public class StaffDto
    {
        public int Id { get; set; }
        public string UserId { get; set; }
        public string Role { get; set; }
        public bool IsActive { get; set; }
        public int? CreatedByStaffId { get; set; }
        public DateTime CreatedAt { get; set; }
        public ICollection<StaffPermissionDto> Permissions { get; set; }
    }
}
