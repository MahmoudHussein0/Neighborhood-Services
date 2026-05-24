using Microsoft.EntityFrameworkCore;
using Neighborhood.Services.Domain.Reviews;
using Neighborhood.Services.Domain.Staffs;

namespace Neighborhood.Services.Infrastructure
{
    public class AppDbContext : DbContext

    {
        public DbSet<Review> Reviews { get; set; }

        public DbSet<ReviewAnalysis> ReviewAnalyses { get; set; }

        public DbSet<Staff> Staffs { get; set; }

        public DbSet<StaffPermission> StaffPermissions { get; set; }
    }
}
