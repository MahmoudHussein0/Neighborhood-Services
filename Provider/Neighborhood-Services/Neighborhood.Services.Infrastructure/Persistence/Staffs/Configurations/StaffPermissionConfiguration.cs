using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Neighborhood.Services.Domain.Staffs;

namespace Neighborhood.Services.Infrastructure.Persistence.Staffs.Configurations
{
    public class StaffPermissionConfiguration : IEntityTypeConfiguration<StaffPermission>
    {
        public void Configure(EntityTypeBuilder<StaffPermission> builder)
        {
            builder.ToTable("StaffPermissions");

            builder.HasKey(p => p.Id);

            builder.Property(p => p.Id)
                .UseIdentityColumn();

            builder.Property(p => p.StaffId)
                .IsRequired();

            builder.Property(p => p.Permission)
                .IsRequired()
                .HasConversion<string>();

            // Prevent duplicate permissions for the same staff member
            builder.HasIndex(p => new { p.StaffId, p.Permission })
                .IsUnique()
                .HasDatabaseName("IX_StaffPermissions_StaffId_Permission");
        }
    }
}
