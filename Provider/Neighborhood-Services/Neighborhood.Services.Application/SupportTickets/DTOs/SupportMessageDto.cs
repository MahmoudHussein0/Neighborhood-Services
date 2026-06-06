namespace Neighborhood.Services.Application.SupportTickets.DTOs
{

    public class SupportMessageDto
    {
        public int Id { get; set; }
        public int TicketId { get; set; }
        public int SenderId { get; set; }
        public string Message { get; set; }
        public string Channel { get; set; }
        public DateTime? ReadAt { get; set; }
        public DateTime CreatedAt { get; set; }
    }
}
