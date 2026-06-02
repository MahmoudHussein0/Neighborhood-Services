using MediatR;
using Neighborhood.Services.Application.SupportTickets.DTOs;
using System;
using System.Collections.Generic;
using System.Text;

namespace Neighborhood.Services.Application.SupportTickets.Commands
{
    public class UpdateSupportMessageCommand : IRequest<SupportMessageDto>
    {
        public int Id { get; set; }
        public DateTime? ReadAt { get; set; }
    }
}
