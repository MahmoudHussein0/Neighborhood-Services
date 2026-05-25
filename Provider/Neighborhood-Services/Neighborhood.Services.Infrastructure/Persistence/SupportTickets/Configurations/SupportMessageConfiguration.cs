using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Neighborhood.Services.Domain.SupportTickets;
using System;
using System.Collections.Generic;
using System.Text;

namespace Neighborhood.Services.Infrastructure.Persistence.SupportTickets.Configurations
{
    public class SupportMessageConfiguration : IEntityTypeConfiguration<SupportMessage>
    {
        public void Configure(EntityTypeBuilder<SupportMessage> builder)
        {
            builder.ToTable("SupportMessages");

            builder.HasKey(m => m.Id);

            builder.Property(m => m.Id)
                .UseIdentityColumn();

            builder.Property(m => m.TicketId)
                .IsRequired();

            builder.Property(m => m.SenderId)
                .IsRequired();

            builder.Property(m => m.Message)
                .IsRequired()
                .HasMaxLength(2000);

            builder.Property(m => m.Channel)
                .IsRequired()
                .HasConversion<int>();

            builder.Property(m => m.IsDeleted)
                .IsRequired()
                .HasDefaultValue(false);

            builder.Property(m => m.ReadAt)
                .IsRequired(false);

            builder.Property(m => m.CreatedAt)
                .IsRequired();

            // Soft delete filter
            builder.HasQueryFilter(m => !m.IsDeleted);

            builder.HasIndex(m => m.TicketId)
                .HasDatabaseName("IX_SupportMessages_TicketId");

            builder.HasIndex(m => m.SenderId)
                .HasDatabaseName("IX_SupportMessages_SenderId");
        }
    }
}
