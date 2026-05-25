using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Neighborhood.Services.Domain.ApplicationUser;

namespace Neighborhood.Services.Infrastructure.Persistence.Users
{
    public class UserConfiguration : IEntityTypeConfiguration<ApplicationUser>
    {
        public void Configure(EntityTypeBuilder<ApplicationUser> builder)
        {
            builder.Property(u => u.FullName)
                .HasMaxLength(100)
                .IsRequired();

            builder.Property(u => u.Age)
                .IsRequired();

            builder.Property(u => u.Photo)
                .HasMaxLength(255);

            builder.Property(u => u.RefferalCode)
                .HasMaxLength(50);

            builder.Property(u => u.IsDeleted)
                .HasDefaultValue(false);

            builder.Property(u => u.IsActive)
                .HasDefaultValue(true);

            builder.Property(u => u.CreatedAt)
                .HasDefaultValueSql("GETUTCDATE()");

            builder.Property(u => u.UpdatedAt)
                .HasDefaultValueSql("GETUTCDATE()");

            builder.Property(u => u.Location)
                .HasColumnType("geography")
                .IsRequired();

            builder.Property(u => u.ApplicationUserRole)
                .HasConversion<string>()
                .HasMaxLength(50)
                .IsRequired();

            builder.HasIndex(u => u.RefferalCode)
                .IsUnique()
                .HasFilter("[RefferalCode] IS NOT NULL AND [RefferalCode] <> ''");
        }
    }
}
