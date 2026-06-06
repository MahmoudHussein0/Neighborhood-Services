using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Neighborhood.Services.Domain.Chatbot;

namespace Neighborhood.Services.Infrastructure.Persistence.Chatbot
{
    public class ChatbotSessionConfiguration : IEntityTypeConfiguration<ChatbotSession>
    {
        public void Configure(EntityTypeBuilder<ChatbotSession> builder)
        {
            builder.HasKey(s => s.Id);

            builder.Property(s => s.UserId)
                .IsRequired()
                .HasMaxLength(450); // matches Identity user id length

            builder.Property(s => s.Title)
                .HasMaxLength(200);

            builder.HasMany(s => s.Messages)
                .WithOne(m => m.ChatbotSession)
                .HasForeignKey(m => m.ChatbotSessionId)
                .OnDelete(DeleteBehavior.Cascade);
        }
    }

    public class ChatbotMessageConfiguration : IEntityTypeConfiguration<ChatbotMessage>
    {
        public void Configure(EntityTypeBuilder<ChatbotMessage> builder)
        {
            builder.HasKey(m => m.Id);

            builder.Property(m => m.Content)
                .IsRequired();

            builder.Property(m => m.Role)
                .HasConversion<string>(); // store "User"/"Assistant" as text
        }
    }
}
