namespace Neighborhood.Services.Application.SupportTickets.DTOs
{

    public class SupportMessageDto
    {
        public int Id { get; set; }

        public int TicketId { get; set; }

        public string? SenderId { get; set; }

        public string? Message { get; set; }

        public string SenderType { get; set; }

        public string Channel { get; set; }

        public DateTime? ReadAt { get; set; }

        public DateTime CreatedAt { get; set; }

        public List<AttachmentDto> Attachments { get; set; }
            = new();
    }
}
