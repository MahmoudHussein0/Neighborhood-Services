using Neighborhood.Services.Domain.Shared;
using System;
using System.Collections.Generic;
using System.Text;

namespace Neighborhood.Services.Domain.SupportTickets
{
    public class MessageAttachment : BaseEntity<int>
    {
        public int MessageId { get; set; }

        public string Url { get; set; } = null!;

        public string PublicId { get; set; } = null!;

        public AttachmentType Type { get; set; }

        public DateTime CreatedAt { get; set; }

        public SupportMessage Message { get; set; } = null!;
    }
}
