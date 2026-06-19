using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Neighborhood.Services.Domain.SupportTickets;

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
                .IsRequired(false)
                .HasMaxLength(450);

            builder.Property(m => m.Message)
                .IsRequired(false)
                .HasMaxLength(2000);

            builder.Property(m => m.Channel)
                .IsRequired()
                .HasConversion<string>();



            builder.Property(m => m.ReadAt)
                .IsRequired(false);

            builder.Property(m => m.CreatedAt)
                .IsRequired();

            builder.Property(m => m.SenderType)
    .IsRequired()
    .HasConversion<string>();

            builder.HasMany(m => m.Attachments)
    .WithOne(a => a.Message)
    .HasForeignKey(a => a.MessageId)
    .OnDelete(DeleteBehavior.Cascade);

            // Soft delete filter
            builder.HasQueryFilter(m => !m.IsDeleted);

            builder.HasIndex(m => m.TicketId)
                .HasDatabaseName("IX_SupportMessages_TicketId");

            builder.HasIndex(m => m.SenderId)
                .HasDatabaseName("IX_SupportMessages_SenderId");
            builder.HasOne(m => m.Sender)
                .WithMany()
                .HasForeignKey(m => m.SenderId)
                .OnDelete(DeleteBehavior.NoAction);

        }
    }
}
