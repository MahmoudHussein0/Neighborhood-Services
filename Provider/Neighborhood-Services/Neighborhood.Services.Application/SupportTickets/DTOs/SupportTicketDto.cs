using System;
using System.Collections.Generic;
using System.Text;

namespace Neighborhood.Services.Application.SupportTickets.DTOs
{
    public class SupportTicketDto
    {
        public int Id { get; set; }
        public string UserId { get; set; }
        public int? BookingId { get; set; }
        public string Subject { get; set; }

        public string Description { get; set; }
        public string Status { get; set; }
        public string Priority { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }
    }
}
