using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Neighborhood.Services.Domain.Conversation;
using System;
using System.Collections.Generic;
using System.Text;

namespace Neighborhood.Services.Infrastructure.Persistence.Conversations
{
    public class ConversationConfiguration : IEntityTypeConfiguration<Conversation>
    {
        public void Configure(EntityTypeBuilder<Conversation> builder)
        {
            builder.HasKey(e => e.Id);

            builder.HasOne(e => e.Booking)
                   .WithOne(e => e.Conversation)
                   .HasForeignKey<Conversation>(e => e.BookingId)
                   .OnDelete(DeleteBehavior.Restrict);

        }
    }
}
