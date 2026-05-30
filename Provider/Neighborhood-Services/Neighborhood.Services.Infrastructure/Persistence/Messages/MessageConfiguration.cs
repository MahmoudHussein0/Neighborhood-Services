using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Neighborhood.Services.Domain.Conversation;
using Neighborhood.Services.Domain.Message;
using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text;

namespace Neighborhood.Services.Infrastructure.Persistence.Messages
{
    public class MessageConfiguration : IEntityTypeConfiguration<Message>
    {
        public void Configure(EntityTypeBuilder<Message> builder)
        {
            //primary key
            builder.HasKey(a => a.Id);

            //relation with foriegn key of user
            builder.HasOne(e => e.Sender)
                .WithMany(e => e.Messages)
                .HasForeignKey(e => e.SenderId)
                .OnDelete(DeleteBehavior.Restrict);

          //relation with foriegn key of conversation
          builder.HasOne(e=>e.Conversation)
                .WithMany(e=>e.Messages)
                .HasForeignKey(e=>e.ConversationId)
                .OnDelete(DeleteBehavior.Restrict);

        //default Values
          
       // builder.Property(e=>e.createdAt).HasDefaultValue(DateTime.UtcNow);

        builder.Property(e => e.isRead).HasDefaultValue(false);

        }
    }
}
