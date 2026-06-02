using MediatR;
using System;
using System.Collections.Generic;
using System.Text;

namespace Neighborhood.Services.Application.SupportTickets.Commands
{
    public class DeleteSupportMessageCommand : IRequest<bool>
    {
        public int Id { get; set; }
    } 
}
