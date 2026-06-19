using Neighborhood.Services.Domain.SupportTickets;

namespace Neighborhood.Services.Application.SupportTickets.DTOs
{
    public class CreateAttachmentDto
    {
        public string Url { get; set; } = null!;

        public string PublicId { get; set; } = null!;

        public AttachmentType Type { get; set; }
    }
}
