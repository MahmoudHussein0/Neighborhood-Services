using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Neighborhood.Services.Domain.Staffs;
using System;
using System.Collections.Generic;
using System.Text;

namespace Neighborhood.Services.Infrastructure.Persistence.Staffs.Configrations
{
    public class StaffConfiguration : IEntityTypeConfiguration<Staff>
    {
        public void Configure(EntityTypeBuilder<Staff> builder)
        {
            builder.ToTable("Staffs");

            builder.HasKey(s => s.Id);

            builder.Property(s => s.Id)
                .UseIdentityColumn();

            builder.Property(s => s.UserId)
                .IsRequired();

            builder.Property(s => s.Role)
                .IsRequired()
                .HasConversion<int>();

            builder.Property(s => s.IsActive)
                .IsRequired()
                .HasDefaultValue(true);

            builder.Property(s => s.CreatedByStaffId)
                .IsRequired(false);

            builder.Property(s => s.CreatedAt)
                .IsRequired();

            // Self-referencing FK: the staff member who created this staff
            builder.HasOne<Staff>()
                .WithMany()
                .HasForeignKey(s => s.CreatedByStaffId)
                .OnDelete(DeleteBehavior.NoAction)
                .IsRequired(false);

            builder.HasMany(s => s.Permissions)
                .WithOne(p => p.Staff)
                .HasForeignKey(p => p.StaffId)
                .OnDelete(DeleteBehavior.NoAction);

            builder.HasIndex(s => s.UserId)
                .IsUnique()
                .HasDatabaseName("IX_Staffs_UserId");

            builder.HasIndex(s => s.Role)
                .HasDatabaseName("IX_Staffs_Role");

            builder.HasIndex(s => s.IsActive)
                .HasDatabaseName("IX_Staffs_IsActive");
        }
    }
}