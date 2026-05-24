using System;
using System.Collections.Generic;
using NetTopologySuite.Geometries;
using System.Text;
using Microsoft.AspNetCore.Identity;

namespace Neighborhood.Services.Domain.ApplicationUser
{
    public class ApplicationUser : IdentityUser
    { 
        public ApplicationUserRole ApplicationUserRole { get; set; }
        public string FullName { get; set; } = string.Empty;
        public int Age { get; set; }
        public string Photo { get; set; } = string.Empty;
        public Point Location { get; set; } = null!;
        public string RefferalCode { get; set; } = string.Empty;
        public bool IsDeleted { get; set; }
        public bool IsActive { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }

    }
}
