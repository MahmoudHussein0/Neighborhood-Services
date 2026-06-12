namespace Neighborhood.Services.Application.Staffs.DTOs
{
    /// <summary>
    /// Data Transfer Object for Staff with ApplicationUser details
    /// Includes FullName and Email from the ApplicationUser entity
    /// </summary>
    public class StaffDto
    {
        public int Id { get; set; }
        public string ApplicationUserId { get; set; }
        public string StaffRole { get; set; }
        public bool IsActive { get; set; }
        public ICollection<StaffPermissionDto> Permissions { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }


        // ApplicationUser Details - Added to return user information
        public string FullName { get; set; }
        public string Email { get; set; }
    }
}
