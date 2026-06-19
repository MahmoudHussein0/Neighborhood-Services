using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Neighborhood.Services.Domain.SupportTickets;
using System;
using System.Collections.Generic;
using System.Text;

namespace Neighborhood.Services.Infrastructure.Persistence.SupportTickets.Configurations
{
    public class MessageAttachmentConfiguration
    : IEntityTypeConfiguration<MessageAttachment>
    {
        public void Configure(EntityTypeBuilder<MessageAttachment> builder)
        {
            builder.ToTable("MessageAttachments");

            builder.HasKey(a => a.Id);

            builder.Property(a => a.Url)
                .IsRequired();

            builder.Property(a => a.PublicId)
                .IsRequired();

            builder.Property(a => a.Type)
                .HasConversion<string>();

            builder.HasOne(a => a.Message)
                .WithMany(m => m.Attachments)
                .HasForeignKey(a => a.MessageId)
                .OnDelete(DeleteBehavior.Cascade);

            builder.HasQueryFilter(a => !a.IsDeleted);
        }
    }
}
