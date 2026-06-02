using Neighborhood.Services.Domain.ApplicationUsers;
using System;
using System.Collections.Generic;
using System.Text;

namespace Neighborhood.Services.Application.Users.DTOs
{
    public class UserDetailsDTO
    {
        public string Id { get; set; } = string.Empty;
        public string FullName { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public int Age { get; set; }
        public string Photo { get; set; } = string.Empty;
        public bool IsActive { get; set; }
        public ApplicationUserRole ApplicationUserRole { get; set; }
    }
}
