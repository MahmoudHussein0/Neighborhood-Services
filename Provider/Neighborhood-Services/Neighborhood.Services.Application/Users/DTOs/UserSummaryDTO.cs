using Neighborhood.Services.Domain.ApplicationUsers;

namespace Neighborhood.Services.Application.Users.DTOs
{
    public class UserSummaryDTO
    {
        public string Id { get; set; } = string.Empty;
        public string FullName { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string Photo { get; set; } = string.Empty;
        public bool IsActive { get; set; }
        public ApplicationUserRole ApplicationUserRole { get; set; }
    }
}
