using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Neighborhood.Services.Domain.Disputes;
using System;
using System.Collections.Generic;
using System.Text;

namespace Neighborhood.Services.Infrastructure.Persistence.Disputes.Configuration
{
    public class DisputeConfiguration : IEntityTypeConfiguration<Dispute>
    {
        public void Configure(EntityTypeBuilder<Dispute> builder)
        {
            builder.ToTable("Disputes");

            builder.HasKey(d => d.Id);

            builder.Property(d => d.Id)
                .UseIdentityColumn();

            builder.Property(d => d.BookingId)
                .IsRequired();

            builder.Property(d => d.RaisedBy)
                .IsRequired();

            builder.Property(d => d.ResolvedByStaffId)
                .IsRequired(false);

            builder.Property(d => d.DisputeType)
                .IsRequired()
                .HasConversion<string>();

            builder.Property(d => d.Reason)
                .IsRequired()
                .HasMaxLength(1000);

            builder.Property(d => d.Resolution)
                .IsRequired(false)
                .HasMaxLength(1000);

            builder.Property(d => d.Status)
                .IsRequired()
                .HasConversion<string>();

            builder.Property(d => d.CreatedAt)
                .IsRequired();

            builder.Property(d => d.ResolvedAt)
                .IsRequired(false);

            // FK to Staff who resolved the dispute
            builder.HasOne<Domain.Staffs.Staff>()
                .WithMany()
                .HasForeignKey(d => d.ResolvedByStaffId)
                .OnDelete(DeleteBehavior.NoAction)
                .IsRequired(false);

            builder.HasIndex(d => d.BookingId)
                .HasDatabaseName("IX_Disputes_BookingId");

            builder.HasIndex(d => d.RaisedBy)
                .HasDatabaseName("IX_Disputes_RaisedBy");

            builder.HasIndex(d => d.Status)
                .HasDatabaseName("IX_Disputes_Status");

            builder.HasIndex(d => d.ResolvedByStaffId)
                .HasDatabaseName("IX_Disputes_ResolvedByStaffId");
        }
    }
}
