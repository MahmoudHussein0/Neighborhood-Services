using MediatR;
using Neighborhood.Services.Application.SupportTickets.DTOs;
using System;
using System.Collections.Generic;
using System.Text;

namespace Neighborhood.Services.Application.SupportTickets.Queries
{
    public class GetSupportTicketByIdQuery : IRequest<SupportTicketDto>
    {
        public int Id { get; set; }
        public GetSupportTicketByIdQuery(int id) => Id = id;
    }
}
