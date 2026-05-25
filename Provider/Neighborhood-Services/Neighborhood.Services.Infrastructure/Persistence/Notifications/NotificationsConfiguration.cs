using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Neighborhood.Services.Domain.Notifications;
using System;
using System.Collections.Generic;
using System.Text;

namespace Neighborhood.Services.Infrastructure.Persistence.Notifications
{
    public class NotificationsConfiguration : IEntityTypeConfiguration<Notification>
    {
        public void Configure(EntityTypeBuilder<Notification> builder)
        {
            builder.HasKey(e => e.Id);

            builder.HasOne(e=>e.User)
                .WithMany(e => e.Notifications)
                .HasForeignKey(e=>e.UserId)
                .OnDelete(DeleteBehavior.Restrict);
        }
    }
}
