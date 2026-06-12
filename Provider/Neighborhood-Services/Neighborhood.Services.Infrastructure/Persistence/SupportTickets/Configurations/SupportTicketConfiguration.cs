using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Neighborhood.Services.Domain.SupportTickets;

namespace Neighborhood.Services.Infrastructure.Persistence.SupportTickets.Configurations
{
    public class SupportTicketConfiguration : IEntityTypeConfiguration<SupportTicket>
    {
        public void Configure(EntityTypeBuilder<SupportTicket> builder)
        {
            builder.ToTable("SupportTickets");

            builder.HasKey(t => t.Id);

            builder.Property(t => t.Id)
                .UseIdentityColumn();

            builder.Property(s => s.UserId)
     .IsRequired()
     .HasMaxLength(450); // ← add this — 450 is the standard Identity FK length
            builder.Property(t => t.BookingId)
                .IsRequired(false);

            builder.Property(t => t.Subject)
                .IsRequired()
                .HasMaxLength(500);

            builder.Property(t => t.Status)
                .IsRequired()
                .HasConversion<string>();
            builder.Property(t => t.Priority)
                .HasConversion<string>();


            builder.Property(t => t.CreatedAt)
                .IsRequired();

            builder.Property(t => t.UpdatedAt)
                .IsRequired();

            // One SupportTicket → Many SupportMessages
            builder.HasMany(t => t.Messages)
                .WithOne(m => m.Ticket)
                .HasForeignKey(m => m.TicketId)
                .OnDelete(DeleteBehavior.NoAction);

            builder.HasOne(t => t.User)
                .WithMany()
                .HasForeignKey(t => t.UserId)
                .OnDelete(DeleteBehavior.NoAction);
            builder.HasOne(t => t.Booking)
                .WithMany()
                .HasForeignKey(t => t.BookingId)
                .OnDelete(DeleteBehavior.NoAction);
            // Soft delete filter
            builder.HasQueryFilter(t => !t.IsDeleted);

            builder.Property(t => t.Description)
    .IsRequired()
    .HasMaxLength(3000);

            builder.HasIndex(t => t.UserId)
                .HasDatabaseName("IX_SupportTickets_UserId");

            builder.HasIndex(t => t.BookingId)
                .HasDatabaseName("IX_SupportTickets_BookingId");

            builder.HasIndex(t => t.Status)
                .HasDatabaseName("IX_SupportTickets_Status");
        }
    }

}
