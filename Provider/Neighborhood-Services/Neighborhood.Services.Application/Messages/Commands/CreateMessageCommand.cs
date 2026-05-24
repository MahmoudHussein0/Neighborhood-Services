using System;
using System.Collections.Generic;
using System.Text;
using MediatR;
using Neighborhood.Services.Application.Messages.DTOs;

namespace Neighborhood.Services.Application.Messages.Commands
{
    public class CreateMessageCommand: IRequest<MessageCreatedDto>
    {
        //foriegn key
        public int ConversationId { get; set; }
        //foriegn Key
        public int SenderId { get; set; }
        public string content { get; set; }
        public bool isRead { get; set; }

        public DateTime createdAt = DateTime.UtcNow;
    }
}
