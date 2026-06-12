using MediatR;
using Neighborhood.Services.Application.SupportTickets.DTOs;

namespace Neighborhood.Services.Application.SupportTickets.Commands
{
    public class CreateSupportTicketCommand : IRequest<SupportTicketDto>
    {


        public string Subject { get; set; }
        public string Description { get; set; }
    }




}
